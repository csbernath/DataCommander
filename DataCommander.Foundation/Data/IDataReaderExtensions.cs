namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using DataCommander.Foundation.Threading;

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
        public static IEnumerable<IDataReader> AsEnumerable( this IDataReader dataReader )
        {
            Contract.Requires<ArgumentNullException>( dataReader != null );

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
        /// <returns></returns>
        public static int Fill( this IDataReader dataReader, DataSet dataSet )
        {
            Contract.Requires<ArgumentNullException>( dataReader != null );
            Contract.Requires<ArgumentNullException>( dataSet != null );

            int rowCount = 0;
            WorkerThread thread = WorkerThread.Current;

            while (!thread.IsStopRequested)
            {
                var table =
                    new DataTable
                    {
                        Locale = CultureInfo.InvariantCulture
                    };

                int count = dataReader.Fill( table );
                rowCount += count;
                dataSet.Tables.Add( table );

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
        /// <returns></returns>
        public static int Fill( this IDataReader dataReader, DataTable dataTable )
        {
            Contract.Requires<ArgumentNullException>( dataReader != null );
            Contract.Requires<ArgumentNullException>( dataTable != null );

            DataTable schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                DataColumnCollection columns = dataTable.Columns;

                if (columns.Count == 0)
                {
                    Database.FillSchema( schemaTable, dataTable );
                }
            }

            int fieldCount = dataReader.FieldCount;
            DataRowCollection rows = dataTable.Rows;
            int rowCount = 0;
            WorkerThread thread = WorkerThread.Current;

            while (!thread.IsStopRequested && dataReader.Read())
            {
                var values = new object[fieldCount];
                dataReader.GetValues( values );
                DataRow row = rows.Add( values );
                row.AcceptChanges();
                rowCount++;
            }

            return rowCount;
        }
    }
}