namespace SqlUtil.Providers.Odbc
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Odbc;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Summary description for OdbcProvider.
    /// </summary>
    class OdbcProvider : IProvider
    {
        public ConnectionBase CreateConnection(string connectionString)
        {
            return new Connection(connectionString);
        }

        public string[] KeyWords
        {
            get
            {
                return null;
            }
        }

        public void SetStandardOutput(IDbCommand command,IStandardOutput standardOutput)
        {
        }

        bool IProvider.IsCommandCancelable
        {
            get
            {
                return false;
            }
        }

        public void DeriveParameters(IDbCommand command)
        {
            OdbcCommand odbcCommand = (OdbcCommand)command;
            OdbcCommandBuilder.DeriveParameters(odbcCommand);
        }

        public DataParameter GetDataParameter(IDataParameter parameter)
        {
            return null;
        }

        public DataTable GetParameterTable(IDataParameterCollection parameters)
        {
            return null;
        }

        public DataTable GetSchemaTable(IDataReader dataReader)
        {
            OdbcDataReader odbcDataReader = (OdbcDataReader)dataReader;
            return odbcDataReader.GetSchemaTable();
        }

        public XmlReader ExecuteXmlReader(IDbCommand command)
        {
            return null;
        }

        public Type ToDataType(int providerType)
        {
            OdbcType odbcType = (OdbcType)providerType;
            Type type;

            switch (odbcType)
            {
                case OdbcType.Double:
                    type = typeof(double);
                    break;

                case OdbcType.VarChar:
                    type = typeof(string);
                    break;

                default:
                    type = typeof(object);
                    break;
            }

            return type;
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            return null;
        }

        //    public void GetValues(IDataReader dataReader,object[] values)
        //    {
        //      dataReader.GetValues(values);
        //    }

        public string ValueToString(object value)
        {
            return value.ToString();
        }

        public DbDataAdapter CreateDataAdapter(string selectCommandText,IDbConnection connection)
        {
            return null;
        }

        public IObjectBrowser ObjectBrowser
        {
            get
            {
                return null;
            }
        }

        public string[] GetIntellisense(ConnectionBase connection,string commandText,int position)
        {
            return  null;
        }

        public void ClearCompletionCache()
        {
        }

        public string GetExceptionMessage(Exception e)
        {
            return e.ToString();
        }
    }
}