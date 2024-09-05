using DataCommander.Providers2;
using DataCommander.Providers2.FieldNamespace;
using Foundation.Assertions;
using System.Data;

namespace DataCommander.Providers.Msi
{
    internal sealed class MsiDataReaderHelper : IDataReaderHelper
    {
        readonly IDataFieldReader[] _dataFieldReaders;

        public MsiDataReaderHelper(MsiDataReader dataReader)
        {
            Assert.IsNotNull(dataReader);

            var view = dataReader.View;
            var index = 0;
            _dataFieldReaders = new IDataFieldReader[view.Columns.Count];

            foreach (var column in view.Columns)
            {
                var dbType = (DbType)column.DBType;
                switch (dbType)
                {
                    case DbType.Int16:
                    case DbType.Int32:
                    case DbType.String:
                        _dataFieldReaders[index] = new DefaultDataFieldReader(dataReader, index);
                        break;

                    case DbType.Binary:
                        _dataFieldReaders[index] = new StreamFieldDataReader(dataReader, index);
                        break;

                    default:
                        break;
                }

                index++;
            }
        }

        #region IDataReaderHelper Members

        int IDataReaderHelper.GetValues(object[] values)
        {
            var count = _dataFieldReaders.Length;

            for (var i = 0; i < count; i++)
            {
                values[i] = _dataFieldReaders[i].Value;
            }

            return count;
        }

        #endregion
    }
}