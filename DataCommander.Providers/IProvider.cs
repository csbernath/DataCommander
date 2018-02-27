using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Xml;
using DataCommander.Providers.Connection;
using Foundation.Data;

namespace DataCommander.Providers
{
    //[ContractClass(typeof (IProviderContract))]
    public interface IProvider
    {
        #region Properties

        string Name { get; }
        DbProviderFactory DbProviderFactory { get; }
        string[] KeyWords { get; }
        bool CanConvertCommandToString { get; }
        bool IsCommandCancelable { get; }
        IObjectExplorer ObjectExplorer { get; }

        #endregion

        #region Methods

        void ClearCompletionCache();
        string CommandToString(IDbCommand command);
        ConnectionBase CreateConnection(string connectionString);
        IDbConnectionStringBuilder CreateConnectionStringBuilder();
        DbDataAdapter CreateDataAdapter(string selectCommandText, IDbConnection connection);
        IDataReaderHelper CreateDataReaderHelper(IDataReader dataReader);

        void CreateInsertCommand(
            DataTable sourceSchemaTable,
            string[] sourceDataTypeNames,
            IDbConnection destinationconnection,
            string destinationTableName,
            out IDbCommand insertCommand,
            out Converter<object, object>[] converters);

        void DeriveParameters(IDbCommand command);
        XmlReader ExecuteXmlReader(IDbCommand command);

        Type GetColumnType(FoundationDbColumn dataColumnSchema);
        string GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName);
        GetCompletionResponse GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position);
        DataParameterBase GetDataParameter(IDataParameter parameter);

        string GetExceptionMessage(Exception exception);
        DataTable GetParameterTable(IDataParameterCollection parameters);
        DataTable GetSchemaTable(IDataReader dataReader);
        List<Statement> GetStatements(string commandText);
        DataSet GetTableSchema(IDbConnection connection, string tableName);
        List<InfoMessage> ToInfoMessages(Exception e);

        #endregion
    }
}