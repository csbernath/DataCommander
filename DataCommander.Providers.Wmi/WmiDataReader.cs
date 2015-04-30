namespace DataCommander.Providers.Wmi
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Management;
    using System.Text;

    internal sealed class WmiDataReader : IDataReader
    {
        private readonly WmiCommand command;
        private readonly ManagementObjectCollection.ManagementObjectEnumerator enumerator;
        private int fieldCount;
        private ManagementBaseObject firstObject;
        private bool firstRead;

        public WmiDataReader(WmiCommand command)
        {
            Contract.Requires(command != null);

            this.command = command;

            var query = new ObjectQuery(command.CommandText);
            var searcher = new ManagementObjectSearcher(command.Connection.Scope, query);
            ManagementObjectCollection objects = searcher.Get();
            this.enumerator = objects.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public bool GetBoolean(int i)
        {
            return false;
        }

        public byte GetByte(int i)
        {
            return 0;
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return 0;
        }

        public char GetChar(int i)
        {
            return '\x00';
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return 0;
        }

        public IDataReader GetData(int i)
        {
            return null;
        }

        public string GetDataTypeName(int i)
        {
            return null;
        }

        public DateTime GetDateTime(int i)
        {
            return DateTime.MinValue;
        }

        public decimal GetDecimal(int i)
        {
            return 0;
        }

        public double GetDouble(int i)
        {
            return 0;
        }

        public Type GetFieldType(int i)
        {
            return null;
        }

        public float GetFloat(int i)
        {
            return 0;
        }

        public Guid GetGuid(int i)
        {
            return Guid.Empty;
        }

        public short GetInt16(int i)
        {
            return 0;
        }

        public int GetInt32(int i)
        {
            return 0;
        }

        public long GetInt64(int i)
        {
            return 0;
        }

        public string GetName(int i)
        {
            return null;
        }

        public int GetOrdinal(string name)
        {
            return 0;
        }

        public string GetString(int i)
        {
            return null;
        }

        public object GetValue(int i)
        {
            return null;
        }

        public int GetValues(object[] values)
        {
            ManagementBaseObject baseObject = this.enumerator.Current;
            PropertyDataCollection.PropertyDataEnumerator enumerator = baseObject.Properties.GetEnumerator();
            int i = 0;

            while (i < values.Length && enumerator.MoveNext())
            {
                PropertyData propertyData = enumerator.Current;
                object value = propertyData.Value;

                if (value == null)
                {
                    values[ i ] = null;
                }
                else
                {
                    switch (propertyData.Type)
                    {
                        case CimType.DateTime:
                            string dmtfDate = value.ToString();
                            var sb = new StringBuilder();
                            sb.Append( dmtfDate.Substring( 0, 4 ) );
                            sb.Append( '-' );
                            sb.Append( dmtfDate.Substring( 4, 2 ) );
                            sb.Append( '-' );
                            sb.Append( dmtfDate.Substring( 6, 2 ) );

                            if (dmtfDate[ 8 ] != '*')
                            {
                                sb.Append( ' ' );
                                sb.Append( dmtfDate.Substring( 8, 2 ) );
                                sb.Append( ':' );
                                sb.Append( dmtfDate.Substring( 10, 2 ) );
                                sb.Append( ':' );
                                sb.Append( dmtfDate.Substring( 12, 2 ) );
                            }

                            values[ i ] = sb.ToString();

                            //              DateTime dateTime = ManagementDateTimeConverter.ToDateTime(dmtfDate);
                            //              DateTime date = dateTime.Date;
                            //              TimeSpan time = dateTime.TimeOfDay;
                            //              string format;
                            //
                            //              if (time.Ticks == 0)
                            //                format = "yyyy-MM-dd";
                            //              else
                            //                format = "yyyy-MM-dd HH:mm:ss.fff";
                            //
                            //              values[i] = dateTime.ToString(format);
                            break;

                        default:
                            values[ i ] = propertyData.Value;
                            break;
                    }
                }

                i++;
            }

            return i;
        }

        public bool IsDBNull(int i)
        {
            return true;
        }

        public int FieldCount
        {
            get
            {
                return this.fieldCount;
            }
        }

        public object this[string name]
        {
            get
            {
                return null;
            }
        }

        public object this[int i]
        {
            get
            {
                return null;
            }
        }

        public void Close()
        {
        }

        public DataTable GetSchemaTable()
        {
            PropertyDataCollection properties = null;

            if (this.firstObject == null)
            {
                bool moveNext = this.enumerator.MoveNext();
                this.firstRead = true;

                if (moveNext)
                {
                    this.firstObject = this.enumerator.Current;
                    properties = this.firstObject.Properties;
                }
                else
                {
                    string query = this.command.CommandText;
                    int index = 0;
                    bool fromFound = false;
                    string className = null;

                    while (true)
                    {
                        int wordStart = QueryTextBox.NextWordStart(query, index);
                        int wordEnd = QueryTextBox.WordEnd(query, wordStart);
                        string word = query.Substring(wordStart, wordEnd - wordStart + 1);

                        if (fromFound)
                        {
                            className = word;
                            break;
                        }
                        else
                        {
                            if (word.ToLower() == "from")
                                fromFound = true;

                            index = wordEnd + 1;
                        }
                    }

                    query = string.Format("SELECT * FROM meta_class WHERE __this ISA '{0}'", className);
                    ObjectQuery objectQuery = new ObjectQuery(query);
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(this.command.Connection.Scope, objectQuery);
                    ManagementObjectCollection objects = searcher.Get();
                    ManagementObjectCollection.ManagementObjectEnumerator enumerator2 = objects.GetEnumerator();
                    enumerator2.MoveNext();
                    properties = enumerator2.Current.Properties;
                }
            }

            DataTable schemaTable = new DataTable();
            schemaTable.Columns.Add("ColumnName", typeof(string));
            schemaTable.Columns.Add("ColumnSize", typeof(int));
            schemaTable.Columns.Add("DataType", typeof(Type));
            schemaTable.Columns.Add("ProviderType", typeof(int));
            schemaTable.Columns.Add("ProviderTypeStr", typeof(string));
            schemaTable.Columns.Add("NumericPrecision", typeof(int));
            schemaTable.Columns.Add("NumericScale", typeof(int));
            schemaTable.Columns.Add("IsArray", typeof(bool));
            object[] values = new object[8];

            foreach (PropertyData propertyData in properties)
            {
                CimType cimType = propertyData.Type;
                Type dataType;
                int size;

                switch (cimType)
                {
                    case CimType.Boolean:
                        dataType = typeof(bool);
                        size = 1;
                        break;

                    case CimType.DateTime:
                        dataType = typeof(DateTime);
                        size = 8;
                        break;

                    case CimType.UInt8:
                        dataType = typeof(byte);
                        size = 1;
                        break;

                    case CimType.UInt16:
                        dataType = typeof(ushort);
                        size = 2;
                        break;

                    case CimType.UInt32:
                        dataType = typeof(uint);
                        size = 4;
                        break;

                    case CimType.UInt64:
                        dataType = typeof(ulong);
                        size = 8;
                        break;

                    case CimType.String:
                        dataType = typeof(string);
                        size = 0;
                        break;

                    default:
                        dataType = typeof(object);
                        size = 0;
                        break;
                }

                int providerType;
                string providerTypeStr;

                if (propertyData.IsArray)
                {
                    providerType = (int)cimType | 0x1000;
                    string typeName = dataType.FullName + "[]";
                    dataType = Type.GetType(typeName);
                    providerTypeStr = cimType.ToString() + "[]";
                }
                else
                {
                    providerType = (int)cimType;
                    providerTypeStr = cimType.ToString();
                }

                values[0] = propertyData.Name;
                values[1] = size;
                values[2] = dataType;
                values[3] = providerType;
                values[4] = providerTypeStr;
                values[7] = propertyData.IsArray;
                schemaTable.Rows.Add(values);
            }

            this.fieldCount = schemaTable.Rows.Count;

            return schemaTable;
        }

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            bool read;

            if (this.command.Cancelled)
            {
                read = false;
            }
            else if (this.firstRead)
            {
                read = this.firstObject != null;
                this.firstRead = false;
            }
            else
            {
                read = this.enumerator.MoveNext();
            }

            return read;
        }

        public int Depth
        {
            get
            {
                return 0;
            }
        }

        public bool IsClosed
        {
            get
            {
                return true;
            }
        }

        public int RecordsAffected
        {
            get
            {
                return 0;
            }
        }
    }
}