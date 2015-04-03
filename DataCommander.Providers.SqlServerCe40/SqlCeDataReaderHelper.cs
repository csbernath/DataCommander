namespace DataCommander.Providers.SqlServerCe
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlServerCe;
    using System.Data.SqlTypes;
    using System.Globalization;
    using DataCommander.Foundation.Data;

    internal sealed class SqlCeDataReaderHelper : IDataReaderHelper
    {
        private SqlCeDataReader dataReader;
        private readonly IDataFieldReader[] dataFieldReaders;

        public SqlCeDataReaderHelper( SqlCeDataReader dataReader )
        {
            this.dataReader = dataReader;
            DataTable schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                DataRowCollection schemaRows = schemaTable.Rows;
                int count = schemaRows.Count;
                this.dataFieldReaders = new IDataFieldReader[ count ];

                for (int i = 0; i < count; i++)
                {
                    DataColumnSchema schemaRow = new DataColumnSchema(schemaRows[i]);
                    SqlCeType sqlCeType = (SqlCeType) schemaRows[ i ][ SchemaTableColumn.ProviderType ];
                    SqlDbType sqlDbType = sqlCeType.SqlDbType;
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

                    this.dataFieldReaders[ i ] = dataFieldReader;
                }
            }
        }

        #region IDataReaderHelper Members

        int IDataReaderHelper.GetValues( object[] values )
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[ i ] = this.dataFieldReaders[ i ].Value;
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
                    bool isDBNull = this.dataReader.IsDBNull( this.columnOrdinal );

                    if (isDBNull)
                    {
                        value = DBNull.Value;
                    }
                    else
                    {
                        SqlDecimal sqlDecimal = this.dataReader.GetSqlDecimal( this.columnOrdinal );
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


                        DecimalField decimalField = new DecimalField( numberFormatInfo, decimalValue, decimalString );
                        value = decimalField;
                    }

                    return value;
                }
            }

            #endregion
        }
    }
}