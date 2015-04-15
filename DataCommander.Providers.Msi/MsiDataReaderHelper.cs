namespace DataCommander.Providers.Msi
{
    using System.Data;
    using System.Diagnostics.Contracts;
    using DataCommander.Providers;
    using Microsoft.Deployment.WindowsInstaller;

    internal sealed class MsiDataReaderHelper : IDataReaderHelper
    {
        IDataFieldReader[] dataFieldReaders;

        public MsiDataReaderHelper( MsiDataReader dataReader )
        {
            Contract.Requires( dataReader != null );
            View view = dataReader.View;
            int index = 0;
            dataFieldReaders = new IDataFieldReader[ view.Columns.Count ];

            foreach (ColumnInfo column in view.Columns)
            {
                DbType dbType = (DbType) column.DBType;
                switch (dbType)
                {
                    case DbType.Int16:
                    case DbType.Int32:
                    case DbType.String:
                        dataFieldReaders[ index ] = new DefaultDataFieldReader( dataReader, index );
                        break;

                    case DbType.Binary:
                        dataFieldReaders[ index ] = new StreamFieldDataReader( dataReader, index );
                        break;

                    default:
                        break;
                }

                index++;
            }
        }

        #region IDataReaderHelper Members

        int IDataReaderHelper.GetValues( object[] values )
        {
            int count = dataFieldReaders.Length;

            for (int i = 0; i < count; i++)
            {
                values[ i ] = dataFieldReaders[ i ].Value;
            }

            return count;
        }

        #endregion
    }
}