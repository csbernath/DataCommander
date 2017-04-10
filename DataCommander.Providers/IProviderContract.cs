namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Xml;
    using DataCommander.Foundation.Data;

    //[ContractClassFor(typeof (IProvider))]
    internal abstract class IProviderContract : IProvider
    {
        #region IProvider Members

        string IProvider.Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        DbProviderFactory IProvider.DbProviderFactory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            throw new NotImplementedException();
        }

        string[] IProvider.KeyWords
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IProvider.CanConvertCommandToString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IProvider.IsCommandCancelable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        void IProvider.DeriveParameters(IDbCommand command)
        {
#if CONTRACTS_FULL
            Contract.Requires(command != null);
#endif
        }

        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
        {
#if CONTRACTS_FULL
            Contract.Requires(parameter != null);
#endif
            return null;
        }

        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
        {
#if CONTRACTS_FULL
            Contract.Requires(parameters != null);
#endif
            return null;
        }

        XmlReader IProvider.ExecuteXmlReader(IDbCommand command)
        {
#if CONTRACTS_FULL
            Contract.Requires(command != null);
#endif
            return null;
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
#if CONTRACTS_FULL
            Contract.Requires(dataReader != null);
#endif
            return null;
        }

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        Type IProvider.GetColumnType(DbColumn dataColumnSchema)
        {
#if CONTRACTS_FULL
            Contract.Requires(dataColumnSchema != null);
#endif
            return null;
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
#if CONTRACTS_FULL
            Contract.Requires(dataReader != null);
#endif
            return null;
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        IObjectExplorer IProvider.ObjectExplorer => null;

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
#if CONTRACTS_FULL
            Contract.Requires(connection != null);
#endif
            return null;
        }

        void IProvider.ClearCompletionCache()
        {
        }

        string IProvider.GetExceptionMessage(Exception exception)
        {
#if CONTRACTS_FULL
            Contract.Requires(exception != null);
#endif
            return null;
        }

        List<InfoMessage> IProvider.ToInfoMessages(Exception e)
        {
            throw new NotImplementedException();
        }

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
        {
            throw new NotImplementedException();
        }

        void IProvider.CreateInsertCommand(DataTable sourceSchemaTable, string[] sourceDataTypeNames, IDbConnection destinationconnection, string destinationTableName,
            out IDbCommand insertCommand, out Converter<object, object>[] converters)
        {
#if CONTRACTS_FULL
            Contract.Ensures(Contract.ValueAtReturn(out insertCommand) != null);
#endif
            insertCommand = null;
            converters = null;
        }

        string IProvider.CommandToString(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        List<Statement> IProvider.GetStatements(string commandText)
        {
            throw new NotImplementedException();
        }

        IDbConnectionStringBuilder IProvider.CreateConnectionStringBuilder()
        {
            throw new NotImplementedException();
        }

#endregion
    }
}