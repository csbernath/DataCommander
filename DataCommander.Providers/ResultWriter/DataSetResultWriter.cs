namespace DataCommander.Providers
{
    using System;
    using System.Data;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Diagnostics;
    using Foundation.Diagnostics.MethodProfiler;

    /// <summary>
    /// Summary description for DataSetResultWriter.
    /// </summary>
    internal sealed class DataSetResultWriter : IResultWriter
    {
        #region Private Fields

        private readonly IResultWriter logResultWriter;
        private QueryForm queryForm;
        private readonly bool showShemaTable;
        private IProvider provider;
        private DataTable dataTable;
        private int rowIndex;

        #endregion

        public DataSetResultWriter(
            Action<InfoMessage> addInfoMessage,
            QueryForm queryForm,
            bool showShemaTable)
        {
            this.logResultWriter = new LogResultWriter(addInfoMessage);
            this.queryForm = queryForm;
            this.showShemaTable = showShemaTable;
        }

        #region Public Properties

        public DataSet DataSet { get; private set; }

        #endregion

        #region IResultWriter Members

        void IResultWriter.Begin(IProvider provider)
        {
            this.logResultWriter.Begin(provider);
            this.provider = provider;
        }

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
        {
            this.logResultWriter.BeforeExecuteReader(command);
        }

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
            this.logResultWriter.AfterExecuteReader(fieldCount);
            this.DataSet = new DataSet();
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
            this.logResultWriter.AfterCloseReader(affectedRows);
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            this.logResultWriter.WriteTableBegin(schemaTable);
            this.CreateTable(schemaTable);
        }

        void IResultWriter.FirstRowReadBegin()
        {
            this.logResultWriter.FirstRowReadBegin();
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
            this.logResultWriter.FirstRowReadEnd(dataTypeNames);
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            MethodProfiler.BeginMethod();
            this.logResultWriter.WriteRows(rows, rowCount);

            try
            {
                var targetRows = this.dataTable.Rows;

                for (int i = 0; i < rowCount; i++)
                {
                    targetRows.Add(rows[i]);
                }

                this.rowIndex += rowCount;
            }
            finally
            {
                MethodProfiler.EndMethod();
            }
        }

        void IResultWriter.WriteTableEnd()
        {
            this.logResultWriter.WriteTableEnd();

            GarbageMonitor.Add("DataSetResultWriter", "System.Data.DataTable", this.dataTable.Rows.Count, this.dataTable);
        }

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
            // TODO TextResultWriter.WriteParameters(parameters, textWriter, queryForm);
        }

        void IResultWriter.End()
        {
            this.logResultWriter.End();

            //int last = this.tableCount - 1;
            //if (last >= 0)
            //{
            //    DataTable table = dataSet.Tables[ last ];
            //    string name = string.Format( "DataTable({0})", table.Rows.Count );
            //    GarbageMonitor.Add( name, dataTable );
            //}
        }

        #endregion

        #region Private Methods

        private void CreateTable(DataTable schemaTable)
        {
            int tableIndex = this.DataSet.Tables.Count;
            string tableName = schemaTable.TableName;
            if (tableName == "SchemaTable")
            {
                tableName = $"Table {tableIndex}";
            }
            if (this.showShemaTable)
            {
                schemaTable.TableName = $"Schema {tableIndex}";
                this.DataSet.Tables.Add(schemaTable);
            }
            this.dataTable = this.DataSet.Tables.Add();
            if (!string.IsNullOrEmpty(tableName))
            {
                this.dataTable.TableName = tableName;
            }
            foreach (DataRow schemaRow in schemaTable.Rows)
            {
                var dataColumnSchema = new DbColumn(schemaRow);
                string columnName = dataColumnSchema.ColumnName;
                int columnSize = dataColumnSchema.ColumnSize;
                Type dataType = this.provider.GetColumnType(dataColumnSchema);

                DataColumn dataColumn;
                int n = 2;
                string columnName2 = columnName;

                while (true)
                {
                    if (this.dataTable.Columns.Contains(columnName2))
                    {
                        columnName2 = columnName + n;
                        n++;
                    }
                    else
                    {
                        columnName = columnName2;

                        if (dataType != null)
                        {
                            dataColumn = this.dataTable.Columns.Add(columnName, dataType);
                        }
                        else
                        {
                            dataColumn = this.dataTable.Columns.Add(columnName);
                        }

                        dataColumn.ExtendedProperties.Add("ColumnName", dataColumnSchema.ColumnName);

                        //dataColumn.AllowDBNull = sr.AllowDBNull == true;                                
                        //dataColumn.Unique = sr.IsUnique == true; // TFS provider does not support this column
                        dataColumn.ExtendedProperties.Add(0, schemaRow["DataType"]);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}