﻿using DataCommander.Providers.FieldNamespace;
using Foundation.Data;

namespace DataCommander.Providers.SqlServerCe40
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlServerCe;
    using System.Globalization;

    internal sealed class SqlCeDataReaderHelper : IDataReaderHelper
    {
        private SqlCeDataReader dataReader;
        private readonly IDataFieldReader[] dataFieldReaders;

        public SqlCeDataReaderHelper( SqlCeDataReader dataReader )
        {
            this.dataReader = dataReader;
            var schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                var schemaRows = schemaTable.Rows;
                var count = schemaRows.Count;
                dataFieldReaders = new IDataFieldReader[ count ];

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

                    dataFieldReaders[ i ] = dataFieldReader;
                }
            }
        }

        #region IDataReaderHelper Members

        int IDataReaderHelper.GetValues( object[] values )
        {
            for (var i = 0; i < values.Length; i++)
            {
                values[ i ] = dataFieldReaders[ i ].Value;
            }

            return values.Length;
        }

        #endregion

        private sealed class SqlDecimalFieldReader : IDataFieldReader
        {
            private static readonly NumberFormatInfo numberFormatInfo;
            private readonly SqlCeDataReader dataReader;
            private readonly int columnOrdinal;

            static SqlDecimalFieldReader()
            {
                numberFormatInfo = (NumberFormatInfo) CultureInfo.InvariantCulture.NumberFormat.Clone();
            }

            public SqlDecimalFieldReader( SqlCeDataReader dataReader, int columnOrdinal )
            {
                this.dataReader = dataReader;
                this.columnOrdinal = columnOrdinal;
            }

            #region IDataFieldReader Members

            object IDataFieldReader.Value
            {
                get
                {
                    object value;
                    var isDBNull = dataReader.IsDBNull( columnOrdinal );

                    if (isDBNull)
                    {
                        value = DBNull.Value;
                    }
                    else
                    {
                        var sqlDecimal = dataReader.GetSqlDecimal( columnOrdinal );
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


                        var decimalField = new DecimalField( numberFormatInfo, decimalValue, decimalString );
                        value = decimalField;
                    }

                    return value;
                }
            }

            #endregion
        }
    }
}