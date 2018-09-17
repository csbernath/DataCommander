using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Globalization;
using DataCommander.Providers.FieldNamespace;
using Foundation.Data;

namespace DataCommander.Providers.SqlServerCe40
{
    internal sealed class SqlCeDataReaderHelper : IDataReaderHelper
    {
        private SqlCeDataReader _dataReader;
        private readonly IDataFieldReader[] _dataFieldReaders;

        public SqlCeDataReaderHelper( SqlCeDataReader dataReader )
        {
            this._dataReader = dataReader;
            var schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                var schemaRows = schemaTable.Rows;
                var count = schemaRows.Count;
                _dataFieldReaders = new IDataFieldReader[ count ];

                for (var i = 0; i < count; i++)
                {
                    var schemaRow = FoundationDbColumnFactory.Create(schemaRows[i]);
                    var sqlCeType = (SqlCeType) schemaRows[ i ][ SchemaTableColumn.ProviderType ];
                    var sqlDbType = sqlCeType.SqlDbType;
                    IDataFieldReader dataFieldReader;

                    switch (sqlDbType)
                    {
                        case SqlDbType.Decimal:
                            dataFieldReader = new SqlDecimalFieldReader( dataReader, i );
                            break;

                        default:
                            dataFieldReader = new DefaultDataFieldReader( dataReader, i );
                            break;
                    }

                    _dataFieldReaders[ i ] = dataFieldReader;
                }
            }
        }

        #region IDataReaderHelper Members

        int IDataReaderHelper.GetValues( object[] values )
        {
            for (var i = 0; i < values.Length; i++)
            {
                values[ i ] = _dataFieldReaders[ i ].Value;
            }

            return values.Length;
        }

        #endregion

        private sealed class SqlDecimalFieldReader : IDataFieldReader
        {
            private static readonly NumberFormatInfo NumberFormatInfo;
            private readonly SqlCeDataReader _dataReader;
            private readonly int _columnOrdinal;

            static SqlDecimalFieldReader()
            {
                NumberFormatInfo = (NumberFormatInfo) CultureInfo.InvariantCulture.NumberFormat.Clone();
            }

            public SqlDecimalFieldReader( SqlCeDataReader dataReader, int columnOrdinal )
            {
                this._dataReader = dataReader;
                this._columnOrdinal = columnOrdinal;
            }

            #region IDataFieldReader Members

            object IDataFieldReader.Value
            {
                get
                {
                    object value;
                    var isDbNull = _dataReader.IsDBNull( _columnOrdinal );

                    if (isDbNull)
                    {
                        value = DBNull.Value;
                    }
                    else
                    {
                        var sqlDecimal = _dataReader.GetSqlDecimal( _columnOrdinal );
                        decimal decimalValue;
                        string decimalString;

                        try
                        {
                            decimalValue = sqlDecimal.Value;
                            decimalString = null;
                        }
                        catch
                        {
                            decimalValue = default(decimal);
                            decimalString = sqlDecimal.ToString();
                        }


                        var decimalField = new DecimalField( NumberFormatInfo, decimalValue, decimalString );
                        value = decimalField;
                    }

                    return value;
                }
            }

            #endregion
        }
    }
}