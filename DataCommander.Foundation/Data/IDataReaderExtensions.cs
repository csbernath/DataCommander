namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;
    using System.Data;
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(dataReader != null);
#endif

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(dataReader != null);
            Contract.Requires<ArgumentNullException>(dataSet != null);
#endif

            var rowCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var table =
                    new DataTable
                    {
                        Locale = CultureInfo.InvariantCulture
                    };

                var count = dataReader.Fill(table, cancellationToken);
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(dataReader != null);
            Contract.Requires<ArgumentNullException>(dataTable != null);
#endif

            var schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                var columns = dataTable.Columns;

                if (columns.Count == 0)
                {
                    Database.FillSchema(schemaTable, dataTable);
                }
            }

            var fieldCount = dataReader.FieldCount;
            var rows = dataTable.Rows;
            var rowCount = 0;

            while (!cancellationToken.IsCancellationRequested && dataReader.Read())
            {
                var values = new object[fieldCount];
                dataReader.GetValues(values);
                var row = rows.Add(values);
                row.AcceptChanges();
                rowCount++;
            }

            return rowCount;
        }
    }
}