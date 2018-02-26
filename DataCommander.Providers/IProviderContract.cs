using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Xml;
using DataCommander.Providers.Connection;
using Foundation.Data;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers
{
    //[ContractClassFor(typeof (IProvider))]
    internal abstract class ProviderContract : IProvider
    {
        #region IProvider Members

        string IProvider.Name => throw new NotImplementedException();

        DbProviderFactory IProvider.DbProviderFactory => throw new NotImplementedException();

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            throw new NotImplementedException();
        }

        string[] IProvider.KeyWords => throw new NotImplementedException();

        bool IProvider.CanConvertCommandToString => throw new NotImplementedException();

        bool IProvider.IsCommandCancelable => throw new NotImplementedException();

        void IProvider.DeriveParameters(IDbCommand command)
        {
            FoundationContract.Requires<ArgumentNullException>(command != null);
        }

        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
        {
            FoundationContract.Requires<ArgumentNullException>(parameter != null);

            return null;
        }

        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
        {
            FoundationContract.Requires<ArgumentNullException>(parameters != null);

            return null;
        }

        XmlReader IProvider.ExecuteXmlReader(IDbCommand command)
        {
            FoundationContract.Requires<ArgumentNullException>(command != null);

            return null;
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
            FoundationContract.Requires<ArgumentNullException>(dataReader != null);

            return null;
        }

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        Type IProvider.GetColumnType(Foundation.Data.DbColumn dataColumnSchema)
        {
            FoundationContract.Requires<ArgumentNullException>(dataColumnSchema != null);

            return null;
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            FoundationContract.Requires<ArgumentNullException>(dataReader != null);

            return null;
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        IObjectExplorer IProvider.ObjectExplorer => null;

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
            FoundationContract.Requires<ArgumentNullException>(connection != null);

            return null;
        }

        void IProvider.ClearCompletionCache()
        {
        }

        string IProvider.GetExceptionMessage(Exception exception)
        {
            FoundationContract.Requires<ArgumentNullException>(exception != null);

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

        void IProvider.CreateInsertCommand(DataTable sourceSchemaTable, string[] sourceDataTypeNames, IDbConnection destinationconnection,
            string destinationTableName,
            out IDbCommand insertCommand, out Converter<object, object>[] converters)
        {
            //FoundationContract.Ensures(Contract.ValueAtReturn(out insertCommand) != null);

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