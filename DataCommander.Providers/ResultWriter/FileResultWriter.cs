namespace DataCommander.Providers
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using DataCommander.Foundation.Text;

    internal sealed class FileResultWriter : IResultWriter
    {
        private readonly TextWriter messageWriter;
        private StreamWriter streamWriter;
        private DataWriterBase[] dataWriters;

        public FileResultWriter(TextWriter messageWriter)
        {
            Contract.Requires(messageWriter != null);

            this.messageWriter = messageWriter;
        }

        #region IResultWriter Members

        void IResultWriter.Begin()
        {
        }

        void IResultWriter.BeforeExecuteReader(IProvider provider, IDbCommand command)
        {
        }

        void IResultWriter.AfterExecuteReader()
        {
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            string path = Path.GetTempFileName();
            this.messageWriter.WriteLine("fileName: {0}", path);
            Encoding encoding = Encoding.UTF8;
            this.streamWriter = new StreamWriter(path, false, encoding, 4096);
            this.streamWriter.AutoFlush = true;
            int count = schemaTable.Rows.Count;
            this.dataWriters = new DataWriterBase[count];
            StringTable st = new StringTable(3);
            st.Columns[2].Align = StringTableColumnAlign.Right;

            for (int i = 0; i < count; i++)
            {
                DataWriterBase dataWriter = null;
                DataRow column = schemaTable.Rows[i];
                Type dataType = (Type)column["DataType"];
                TypeCode typeCode = Type.GetTypeCode(dataType);
                string dataTypeName;
                int length;

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        length = 1;
                        dataTypeName = "bit";
                        dataWriter = new BooleanDataWriter();
                        dataWriter.Init(5);
                        break;

                    case TypeCode.DateTime:
                        length = 21; // yyyyMMdd HH:mm:ss.fff
                        dataTypeName = "datetime";
                        dataWriter = new DateTimeDataWriter();
                        dataWriter.Init(21);
                        break;

                    case TypeCode.Decimal:
                        short precision = (short)column["NumericPrecision"];
                        short scale = (short)column["NumericScale"];
                        length = precision + 1; // +/- sign

                        // decimal separator
                        if (scale > 0)
                        {
                            length++;
                            dataTypeName = $"decimal({precision},{scale})";
                        }
                        else
                        {
                            dataTypeName = $"decimal({precision})";
                        }

                        dataWriter = new DecimalDataWriter();
                        dataWriter.Init(length);
                        break;

                    case TypeCode.Int16:
                        length = short.MinValue.ToString().Length;
                        dataTypeName = "smallint";
                        dataWriter = new DecimalDataWriter();
                        dataWriter.Init(length);
                        break;

                    case TypeCode.Int32:
                        length = int.MinValue.ToString().Length;
                        dataTypeName = "int";
                        dataWriter = new DecimalDataWriter();
                        dataWriter.Init(length);
                        break;

                    case TypeCode.String:
                        length = (int)column["ColumnSize"];
                        length = Math.Min(1024, length);
                        dataTypeName = $"varchar({length})";
                        dataWriter = new StringDataWriter();
                        dataWriter.Init(length);
                        break;

                    case TypeCode.Object:
                        if (dataType == typeof (Guid))
                        {
                            length = Guid.Empty.ToString().Length;
                            dataTypeName = "uniqueidentifier";
                        }
                        else
                        {
                            throw new NotImplementedException(dataType.ToString());
                        }

                        break;

                    default:
                        throw new NotImplementedException(typeCode.ToString());
                }

                this.dataWriters[i] = dataWriter;

                StringTableRow row = st.NewRow();
                row[0] = (string)column[SchemaTableColumn.ColumnName];
                row[1] = dataTypeName;
                row[2] = length.ToString();
                st.Rows.Add(row);
            }

            this.messageWriter.WriteLine(st);
        }

        public void FirstRowReadBegin()
        {
            // TODO:  Add FileResultWriter.FirstRowReadBegin implementation
        }

        public void FirstRowReadEnd(string[] dataTypeNames)
        {
            // TODO:  Add FileResultWriter.FirstRowReadEnd implementation
        }

        public void WriteRows(object[][] rows, int rowCount)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < rowCount; i++)
            {
                object[] row = rows[i];

                for (int j = 0; j < row.Length; j++)
                {
                    string s = this.dataWriters[j].ToString(row[j]);
                    sb.Append(s);
                }

                sb.Append("\r\n");
            }

            this.streamWriter.Write(sb);
        }

        public void WriteTableEnd()
        {
            this.streamWriter.Close();
            this.streamWriter = null;
            this.dataWriters = null;
        }

        public void WriteParameters(IDataParameterCollection parameters)
        {
            // TODO:  Add FileResultWriter.WriteParameters implementation
        }

        public void WriteEnd()
        {
            // TODO:  Add FileResultWriter.WriteEnd implementation
        }

        void IResultWriter.End()
        {
        }

        #endregion
    }
}