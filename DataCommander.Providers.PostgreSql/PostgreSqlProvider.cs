namespace DataCommander.Providers.PostgreSql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using DataCommander.Providers;
    using Foundation.Data;
    using Npgsql;

    internal sealed class PostgreSqlProvider : IProvider
    {
        string IProvider.Name
        {
            get
            {
                return "PostgreSql";
            }
        }

        DbProviderFactory IProvider.DbProviderFactory
        {
            get
            {
                return NpgsqlFactory.Instance;
            }
        }

        string[] IProvider.KeyWords
        {
            get
            {
                // TODO
                return null;
            }
        }

        bool IProvider.CanConvertCommandToString
        {
            get { throw new NotImplementedException(); }
        }

        bool IProvider.IsCommandCancelable
        {
            get { throw new NotImplementedException(); }
        }

        IObjectExplorer IProvider.ObjectExplorer
        {
            get
            {
                // TODO
                return null;
            }
        }

        void IProvider.ClearCompletionCache()
        {
            throw new NotImplementedException();
        }

        string IProvider.CommandToString(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            return new Connection(connectionString);
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            return new PostgreSqlDataReaderHelper((NpgsqlDataReader)dataReader);
        }

        void IProvider.CreateInsertCommand(DataTable sourceSchemaTable, string[] sourceDataTypeNames, IDbConnection destinationconnection, string destinationTableName, out IDbCommand insertCommand, out Converter<object, object>[] converters)
        {
            throw new NotImplementedException();
        }

        void IProvider.DeriveParameters(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        System.Xml.XmlReader IProvider.ExecuteXmlReader(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        Type IProvider.GetColumnType(DataColumnSchema dataColumnSchema)
        {
            // TODO
            return typeof (object);
        }

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
        {
            throw new NotImplementedException();
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
            return exception.ToString();
        }

        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
        {
            throw new NotImplementedException();
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
            throw new NotImplementedException();
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