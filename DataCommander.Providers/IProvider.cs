namespace DataCommander.Providers
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.Contracts;
    using System.Xml;

    [ContractClass( typeof( IProviderContract ) )]
    public interface IProvider
    {
        string Name
        {
            get;
        }

        DbProviderFactory DbProviderFactory
        {
            get;
        }

        ConnectionBase CreateConnection( string connectionString );

        string[] KeyWords
        {
            get;
        }

        bool CanConvertCommandToString
        {
            get;
        }

        bool IsCommandCancelable
        {
            get;
        }

        void DeriveParameters( IDbCommand command );

        DataParameterBase GetDataParameter( IDataParameter parameter );

        DataTable GetParameterTable( IDataParameterCollection parameters );

        XmlReader ExecuteXmlReader( IDbCommand command );

        DataTable GetSchemaTable( IDataReader dataReader );

        DataSet GetTableSchema( IDbConnection connection, string tableName );

        Type GetColumnType( DataRow schemaRow );

        IDataReaderHelper CreateDataReaderHelper( IDataReader dataReader );

        DbDataAdapter CreateDataAdapter( string selectCommandText, IDbConnection connection );

        IObjectExplorer ObjectExplorer
        {
            get;
        }

        GetCompletionResponse GetCompletion( ConnectionBase connection, IDbTransaction transaction, string text, int position );

        void ClearCompletionCache();

        string GetExceptionMessage( Exception e );
        InfoMessage[] ToInfoMessages( Exception e );

        string GetColumnTypeName( IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName );

        void CreateInsertCommand(
            DataTable sourceSchemaTable,
            string[] sourceDataTypeNames,
            IDbConnection destinationconnection,
            string destinationTableName,
            out IDbCommand insertCommand,
            out Converter<object, object>[] converters );

        string CommandToString( IDbCommand command );

        string[] GetStatements( string commandText );
    }
}