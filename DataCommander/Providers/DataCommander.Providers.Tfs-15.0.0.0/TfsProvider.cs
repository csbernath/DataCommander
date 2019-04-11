using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Xml;
using DataCommander.Providers.Connection;
using DataCommander.Providers.Query;
using Foundation.Data;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace DataCommander.Providers.Tfs
{
    public sealed class TfsProvider : IProvider
    {
        private TfsObjectExplorer _objectBrowser;

        static TfsProvider()
        {
            var parameters = new TfsParameterCollection();
            parameters.AddStringInput("serverPath", false, null);
            parameters.AddStringInput("localPath", true, null);

            TfsDataReaderFactory.Add("get", parameters, command => new TfsDownloadDataReader(command));

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("path", false, null);
            parameters.AddValueTypeInput("recursion", RecursionType.OneLevel);

            TfsDataReaderFactory.Add("dir", parameters, command => new TfsGetItemsDataReader(command));

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("path", false, null);
            parameters.AddValueTypeInput("recursion", RecursionType.OneLevel);

            TfsDataReaderFactory.Add("extendeddir", parameters, command => new TfsGetExtendedItemsDataReader(command));

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("path", false, null);
            parameters.AddValueTypeInput("recursion", RecursionType.Full);
            parameters.AddStringInput("user", true, null);
            parameters.AddInt32Input("maxCount", true, int.MaxValue);
            parameters.AddBooleanInput("includeChanges", true, false);
            parameters.AddBooleanInput("slotMode", true, false);

            TfsDataReaderFactory.Add("history", parameters, command => new TfsQueryHistoryDataReader(command));

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("path", false, null);
            parameters.AddValueTypeInput("recursion", RecursionType.Full);
            parameters.AddStringInput("workspace", true, null);
            parameters.AddStringInput("user", true, null);

            TfsDataReaderFactory.Add("status", parameters, command => new TfsQueryPendingSetsDataReader(command));

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("workspace", true, null);
            parameters.AddStringInput("owner", true, null);
            parameters.AddStringInput("computer", true, null);

            TfsDataReaderFactory.Add("workspaces", parameters, command => new TfsQueryWorkspacesDataReader(command));

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("path", false, null);
            parameters.AddStringInput("user", true, null);
            parameters.AddInt32Input("maxCount", true, int.MaxValue);
            parameters.AddBooleanInput("slotMode", true, false);
            parameters.AddStringInput("localPath", true, null);

            TfsDataReaderFactory.Add("getversions", parameters, command => new TfsDownloadItemVersionsDataReader(command));
        }

        #region IProvider Members

        string IProvider.Name => "Tfs-15.0.0.0";
        DbProviderFactory IProvider.DbProviderFactory => TfsProviderFactory.Instance;

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            var connectionStringBuilder = (IDbConnectionStringBuilder) new TfsConnectionStringBuilder();
            connectionStringBuilder.ConnectionString = connectionString;

            connectionStringBuilder.TryGetValue(ConnectionStringKeyword.DataSource, out var value);
            var uriString = (string) value;
            var uri = new Uri(uriString);
            return new TfsConnection(uri);
        }

        string[] IProvider.KeyWords
        {
            get
            {
                var names = new List<string>();

                foreach (var name in TfsDataReaderFactory.Dictionary.Keys)
                    names.Add(name);

                return names.ToArray();
            }
        }

        bool IProvider.CanConvertCommandToString => false;

        bool IProvider.IsCommandCancelable => true;

        void IProvider.DeriveParameters(IDbCommand command)
        {
            var tfsCommand = (TfsCommand) command;

            var contains = TfsDataReaderFactory.Dictionary.TryGetValue(tfsCommand.CommandText, out var info);

            if (contains)
            {
                foreach (var parameter in info.Parameters)
                {
                    var clone = new TfsParameter(parameter.ParameterName, parameter.Type, parameter.DbType, parameter.Direction, parameter.IsNullable,
                        parameter.DefaultValue);
                    tfsCommand.Parameters.Add(clone);
                }
            }
        }

        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
        {
            var tfsParameter = (TfsParameter) parameter;
            return new TfsDataParameter(tfsParameter);
        }

        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
        {
            var tfsParameters = (TfsParameterCollection) parameters;
            var table = new DataTable();
            var columns = table.Columns;
            columns.Add("ParameterName", typeof(string));
            columns.Add("DbType", typeof(DbType));
            columns.Add("Direction", typeof(ParameterDirection));
            columns.Add("IsNullable", typeof(bool));
            columns.Add("DefaultValue");
            columns.Add("Value");
            var rows = table.Rows;

            foreach (var tfsParameter in tfsParameters)
            {
                rows.Add(new[]
                {
                    tfsParameter.ParameterName,
                    tfsParameter.DbType,
                    tfsParameter.Direction,
                    tfsParameter.IsNullable,
                    tfsParameter.DefaultValue,
                    tfsParameter.Value
                });
            }

            return table;
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader) => new DataTable();
        GetTableSchemaResult IProvider.GetTableSchema(IDbConnection connection, string tableName) => throw new NotImplementedException();

        Type IProvider.GetColumnType(FoundationDbColumn dataColumnSchema)
        {
            var dbType = (DbType) dataColumnSchema.ProviderType;
            Type type;

            switch (dbType)
            {
                case DbType.DateTime:
                    type = typeof(DateTime);
                    break;

                case DbType.String:
                    type = typeof(string);
                    break;

                case DbType.Int32:
                    type = typeof(int);
                    break;

                default:
                    type = typeof(object);
                    break;
            }

            return type;
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            var tfsDataReader = (TfsDataReader) dataReader;
            return new TfsDataReaderHelper(tfsDataReader);
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        public IObjectExplorer CreateObjectExplorer() => new TfsObjectExplorer();

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
            var response = new GetCompletionResponse();
            string[] values = null;
            var sqlStatement = new SqlParser(text);
            var tokens = sqlStatement.Tokens;

            if (tokens.Count > 0)
            {
                sqlStatement.FindToken(position, out var previousToken, out var currentToken);
                if (currentToken != null)
                {
                    response.StartPosition = currentToken.StartPosition;
                    response.Length = currentToken.EndPosition - currentToken.StartPosition + 1;
                }
                else
                {
                    response.StartPosition = position;
                    response.Length = 0;
                }

                if (previousToken != null)
                {
                    var token = previousToken;

                    if (token.Type == TokenType.KeyWord && string.Compare(token.Value, "exec", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        var names = new List<string>();

                        foreach (var name in TfsDataReaderFactory.Dictionary.Keys)
                        {
                            names.Add(name);
                        }

                        values = names.ToArray();
                    }
                    else if (tokens.Count > 1)
                    {
                        token = tokens[0];

                        if (token.Type == TokenType.KeyWord && string.Compare(token.Value, "exec", StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            //token = tokens[1];
                            //TfsDataReaderFactory.DataReaderInfo info;
                            //bool contains = TfsDataReaderFactory.Dictionary.TryGetValue(token.Value, out info);

                            //if (contains)
                            //{
                            //    TfsParameterCollection parameters = info.Parameters;
                            //    TfsParameter parameter = parameters[index - 2];
                            //    Type type = parameter.Type;

                            //    if (type.IsEnum)
                            //    {
                            //        string[] names = Enum.GetNames(type);
                            //        values = names;
                            //    }
                            //}
                            var command = sqlStatement.CreateCommand(this, connection, CommandType.StoredProcedure, 0);
                            var contains = TfsDataReaderFactory.Dictionary.TryGetValue(command.CommandText, out var info);
                            if (contains)
                            {
                                var parameters = info.Parameters;
                                var parameterIndex = previousToken.Index / 2;
                                if (parameterIndex < parameters.Count)
                                {
                                    var parameter = parameters[parameterIndex];
                                    var type = parameter.Type;
                                    if (type.IsEnum)
                                    {
                                        var names = Enum.GetNames(type);
                                        values = names;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            response.Items = values.Select(value => (IObjectName) new ObjectName(value)).ToList();
            return response;
        }

        void IProvider.ClearCompletionCache() => throw new NotImplementedException();
        List<InfoMessage> IProvider.ToInfoMessages(Exception exception) => throw new NotImplementedException();
        string IProvider.GetExceptionMessage(Exception e) => e.ToString();

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName) => throw new NotImplementedException();

        void IProvider.CreateInsertCommand(
            DataTable sourceSchemaTable,
            string[] sourceDataTypeNames,
            IDbConnection destinationconnection,
            string destinationTableName,
            out IDbCommand insertCommand,
            out Converter<object, object>[] converters) => throw new NotImplementedException();

        string IProvider.CommandToString(IDbCommand command) => throw new NotImplementedException();

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

        IDbConnectionStringBuilder IProvider.CreateConnectionStringBuilder() => new TfsConnectionStringBuilder();

        #endregion
    }
}