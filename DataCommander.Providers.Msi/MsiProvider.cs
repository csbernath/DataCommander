namespace DataCommander.Providers.Msi
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Xml;
    using DataCommander.Foundation.Data;

    public sealed class MsiProvider : IProvider
    {
        #region IProvider Members

        bool IProvider.CanConvertCommandToString
        {
            get
            {
                return false;
            }
        }

        //public Type ToDataType(int providerType, int columnSize)
        //{
        //    DbType dbType = (DbType)providerType;
        //    Type type;

        //    switch (dbType)
        //    {
        //        case DbType.Int16:
        //            type = typeof (Int16);
        //            break;

        //        case DbType.Int32:
        //            type = typeof (Int32);
        //            break;

        //        case DbType.String:
        //            type = typeof (String);
        //            break;

        //        case DbType.Binary:
        //            type = typeof (StreamField);
        //            break;

        //        default:
        //            throw new NotImplementedException();
        //    }

        //    return type;
        //}

        //public IDataReaderHelper CreateDataReaderHelper(System.Data.IDataReader dataReader)
        //{
        //    MsiDataReader msiDataReader = (MsiDataReader)dataReader;
        //    return new MsiDataReaderHelper(msiDataReader);
        //}

        public void ClearCompletionCache()
        {
            throw new NotImplementedException();
        }

        public string GetExceptionMessage(Exception e)
        {
            return e.ToString();
        }

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
        {
            throw new NotImplementedException();
        }

        void IProvider.CreateInsertCommand(
            DataTable sourceSchemaTable,
            string[] sourceDataTypeNames,
            IDbConnection destinationconnection,
            string destinationTableName,
            out IDbCommand insertCommand,
            out Converter<object, object>[] converters)
        {
            throw new NotImplementedException();
        }

        string IProvider.CommandToString(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        #endregion

        string IProvider.Name
        {
            get
            {
                return "Msi";
            }
        }

        DbProviderFactory IProvider.DbProviderFactory
        {
            get
            {
                return MsiProviderFactory.Instance;
            }
        }

        string[] IProvider.KeyWords
        {
            get
            {
                return null;
            }
        }

        bool IProvider.IsCommandCancelable
        {
            get
            {
                return false;
            }
        }

        IObjectExplorer IProvider.ObjectExplorer
        {
            get
            {
                return new MsiObjectExplorer();
            }
        }

        void IProvider.ClearCompletionCache()
        {
            throw new NotImplementedException();
        }

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            return new MsiProviderConnection(connectionString);
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            var msiDataReader = (MsiDataReader)dataReader;
            return new MsiDataReaderHelper(msiDataReader);
        }

        void IProvider.DeriveParameters(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        XmlReader IProvider.ExecuteXmlReader(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        Type IProvider.GetColumnType(DataColumnSchema dataColumnSchema)
        {
            return typeof (object);
        }

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
            throw new NotImplementedException();
        }

        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
        {
            throw new NotImplementedException();
        }

        string IProvider.GetExceptionMessage(Exception exception)
        {
            throw new NotImplementedException();
        }

        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
        {
            throw new NotImplementedException();
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
            return dataReader.GetSchemaTable();
        }

        List<Statement> IProvider.GetStatements(string commandText)
        {
            return new List<Statement>
            {
                new Statement
                {
                    LineIndex = 0,
                    CommandText = commandText
                }
            };
        }

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        List<InfoMessage> IProvider.ToInfoMessages(Exception e)
        {
            throw new NotImplementedException();
        }
    }
}