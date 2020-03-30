using System;
using System.Data;
using System.Data.SqlClient;
using DataCommander.Providers.FieldNamespace;
using DataCommander.Providers2;
using DataCommander.Providers2.FieldNamespace;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.FieldReader
{
    internal sealed class SqlDataReaderHelper : IDataReaderHelper
    {
        private readonly IDataFieldReader[] _dataFieldReaders;
        private SqlDataReader _sqlDataReader;

        public SqlDataReaderHelper(IDataReader dataReader)
        {
            _sqlDataReader = (SqlDataReader) dataReader;
            var schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                var rows = schemaTable.Rows;
                var count = rows.Count;
                _dataFieldReaders = new IDataFieldReader[count];

                for (var i = 0; i < count; ++i)
                    _dataFieldReaders[i] = CreateDataFieldReader(dataReader, FoundationDbColumnFactory.Create(rows[i]));
            }
        }

        int IDataReaderHelper.GetValues(object[] values)
        {
            for (var i = 0; i < _dataFieldReaders.Length; i++) values[i] = _dataFieldReaders[i].Value;

            return _dataFieldReaders.Length;
        }

        private static IDataFieldReader CreateDataFieldReader(IDataRecord dataRecord, FoundationDbColumn dataColumnSchema)
        {
            var columnOrdinal = dataColumnSchema.ColumnOrdinal;
            var providerType = (SqlDbType) dataColumnSchema.ProviderType;
            IDataFieldReader dataFieldReader;

            switch (providerType)
            {
                case SqlDbType.BigInt:
                case SqlDbType.Bit:
                case SqlDbType.Udt:
                case SqlDbType.Int:
                case SqlDbType.SmallInt:
                case SqlDbType.TinyInt:
                case SqlDbType.UniqueIdentifier:
                case SqlDbType.Real: //
                    dataFieldReader = new DefaultDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Float: //
                    dataFieldReader = new DoubleFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.NText:
                    var columnSize = dataColumnSchema.ColumnSize;

                    if (columnSize <= SqlServerProvider.ShortStringSize)
                        dataFieldReader = new ShortStringFieldReader(dataRecord, columnOrdinal, providerType);
                    else
                        dataFieldReader = new LongStringFieldReader(dataRecord, columnOrdinal);

                    break;

                case SqlDbType.Binary:
                case SqlDbType.VarBinary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                    dataFieldReader = new BinaryDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Decimal:
                    dataFieldReader = new DefaultDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.SmallDateTime:
                    dataFieldReader = new SmallDateTimeDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    dataFieldReader = new DateTimeDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.DateTimeOffset:
                    dataFieldReader = new DateTimeOffsetDataFieldReader(dataRecord, columnOrdinal);
                    break;


                case SqlDbType.Time:
                    dataFieldReader = new DefaultDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Money:
                    dataFieldReader = new MoneyDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Variant:
                    dataFieldReader = new VariantDataFieldReader(dataRecord, columnOrdinal);
                    break;

                //                case SqlDbType.Timestamp:
                //                    dataFieldReader = new TimeStampDataFieldReader(dataRecord,columnOrdinal);
                //                    break;

                case SqlDbType.Xml:
                    dataFieldReader = new LongStringFieldReader(dataRecord, columnOrdinal);
                    break;

                default:
                    throw new Exception();
            }

            return dataFieldReader;
        }
    }
}