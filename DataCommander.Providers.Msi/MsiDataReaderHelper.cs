namespace DataCommander.Providers.Msi
{
    using System.Data;
    using Field;

    internal sealed class MsiDataReaderHelper : IDataReaderHelper
    {
        IDataFieldReader[] dataFieldReaders;

        public MsiDataReaderHelper( MsiDataReader dataReader )
        {
#if CONTRACTS_FULL
            Contract.Requires( dataReader != null );
#endif
            var view = dataReader.View;
            var index = 0;
            dataFieldReaders = new IDataFieldReader[ view.Columns.Count ];

            foreach (var column in view.Columns)
            {
                var dbType = (DbType) column.DBType;
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
            var count = dataFieldReaders.Length;

            for (var i = 0; i < count; i++)
            {
                values[ i ] = dataFieldReaders[ i ].Value;
            }

            return count;
        }

#endregion
    }
}