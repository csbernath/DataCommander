namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.Contracts;
    using System.Xml;
    using DataCommander.Foundation.Data;

    [ContractClassFor(typeof (IProvider))]
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
            Contract.Requires(command != null);
        }

        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
        {
            Contract.Requires(parameter != null);
            return null;
        }

        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
        {
            Contract.Requires(parameters != null);
            return null;
        }

        XmlReader IProvider.ExecuteXmlReader(IDbCommand command)
        {
            Contract.Requires(command != null);
            return null;
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
            Contract.Requires(dataReader != null);
            return null;
        }

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        Type IProvider.GetColumnType(DataColumnSchema dataColumnSchema)
        {
            Contract.Requires(dataColumnSchema != null);
            return null;
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            Contract.Requires(dataReader != null);
            return null;
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        IObjectExplorer IProvider.ObjectExplorer
        {
            get
            {
                return null;
            }
        }

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
            Contract.Requires(connection != null);

            return null;
        }

        void IProvider.ClearCompletionCache()
        {
        }

        string IProvider.GetExceptionMessage(Exception exception)
        {
            Contract.Requires(exception != null);

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
            Contract.Ensures(Contract.ValueAtReturn(out insertCommand) != null);
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

        #endregion
    }
}