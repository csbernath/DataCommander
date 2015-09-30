namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public static class IDataReaderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public static IEnumerable<IDataReader> AsEnumerable(this IDataReader dataReader)
        {
            Contract.Requires<ArgumentNullException>(dataReader != null);

            while (dataReader.Read())
            {
                yield return dataReader;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="dataSet"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static int Fill(this IDataReader dataReader, DataSet dataSet, CancellationToken cancellationToken)
        {
            Contract.Requires<ArgumentNullException>(dataReader != null);
            Contract.Requires<ArgumentNullException>(dataSet != null);

            int rowCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var table =
                    new DataTable
                    {
                        Locale = CultureInfo.InvariantCulture
                    };

                int count = dataReader.Fill(table, cancellationToken);
                rowCount += count;
                dataSet.Tables.Add(table);

                if (!dataReader.NextResult())
                {
                    break;
                }
            }

            return rowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="dataTable"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static int Fill(this IDataReader dataReader, DataTable dataTable, CancellationToken cancellationToken)
        {
            Contract.Requires<ArgumentNullException>(dataReader != null);
            Contract.Requires<ArgumentNullException>(dataTable != null);

            DataTable schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                DataColumnCollection columns = dataTable.Columns;

                if (columns.Count == 0)
                {
                    Database.FillSchema(schemaTable, dataTable);
                }
            }

            int fieldCount = dataReader.FieldCount;
            DataRowCollection rows = dataTable.Rows;
            int rowCount = 0;

            while (!cancellationToken.IsCancellationRequested && dataReader.Read())
            {
                var values = new object[fieldCount];
                dataReader.GetValues(values);
                DataRow row = rows.Add(values);
                row.AcceptChanges();
                rowCount++;
            }

            return rowCount;
        }
    }
}