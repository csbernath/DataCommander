namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Xml;
    using DataCommander.Foundation.Data;
    using Microsoft.TeamFoundation.VersionControl.Client;

    public sealed class TfsProvider : IProvider
    {
        private TfsObjectExplorer objectBrowser;

        static TfsProvider()
        {
            var parameters = new TfsParameterCollection();
            parameters.AddStringInput("serverPath", false, null);
            parameters.AddStringInput("localPath", true, null);

            TfsDataReaderFactory.Add("get", parameters, delegate(TfsCommand command)
            {
                return new TfsDownloadDataReader(command);
            });

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("path", false, null);
            parameters.AddValueTypeInput("recursion", RecursionType.OneLevel);

            TfsDataReaderFactory.Add("dir", parameters, delegate(TfsCommand command)
            {
                return new TfsGetItemsDataReader(command);
            });

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("path", false, null);
            parameters.AddValueTypeInput("recursion", RecursionType.OneLevel);

            TfsDataReaderFactory.Add("extendeddir", parameters, delegate(TfsCommand command)
            {
                return new TfsGetExtendedItemsDataReader(command);
            });

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("path", false, null);
            parameters.AddValueTypeInput("recursion", RecursionType.Full);
            parameters.AddStringInput("user", true, null);
            parameters.AddInt32Input("maxCount", true, int.MaxValue);
            parameters.AddBooleanInput("includeChanges", true, false);
            parameters.AddBooleanInput("slotMode", true, false);

            TfsDataReaderFactory.Add("history", parameters, delegate(TfsCommand command)
            {
                return new TfsQueryHistoryDataReader(command);
            });

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("path", false, null);
            parameters.AddValueTypeInput("recursion", RecursionType.Full);
            parameters.AddStringInput("workspace", true, null);
            parameters.AddStringInput("user", true, null);

            TfsDataReaderFactory.Add("status", parameters, delegate(TfsCommand command)
            {
                return new TfsQueryPendingSetsDataReader(command);
            });

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("workspace", true, null);
            parameters.AddStringInput("owner", true, null);
            parameters.AddStringInput("computer", true, null);

            TfsDataReaderFactory.Add("workspaces", parameters, delegate(TfsCommand command)
            {
                return new TfsQueryWorkspacesDataReader(command);
            });

            parameters = new TfsParameterCollection();
            parameters.AddStringInput("path", false, null);
            parameters.AddStringInput("user", true, null);
            parameters.AddInt32Input("maxCount", true, int.MaxValue);
            parameters.AddBooleanInput("slotMode", true, false);
            parameters.AddStringInput("localPath", true, null);

            TfsDataReaderFactory.Add("getversions", parameters, delegate(TfsCommand command)
            {
                return new TfsDownloadItemVersionsDataReader(command);
            });
        }

        #region IProvider Members

        string IProvider.Name
        {
            get
            {
                return "Tfs-11.0.0.0";
            }
        }

        DbProviderFactory IProvider.DbProviderFactory
        {
            get
            {
                return TfsProviderFactory.Instance;
            }
        }

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            var dbConnectionStringBuilder = new DbConnectionStringBuilder();
            dbConnectionStringBuilder.ConnectionString = connectionString;
            string uriString = (string)dbConnectionStringBuilder[ConnectionStringProperty.Server];
            var uri = new Uri(uriString);
            return new TfsConnection(uri);
        }

        string[] IProvider.KeyWords
        {
            get
            {
                var names = new List<string>();

                foreach (string name in TfsDataReaderFactory.Dictionary.Keys)
                {
                    names.Add(name);
                }

                return names.ToArray();
            }
        }

        bool IProvider.CanConvertCommandToString
        {
            get
            {
                return false;
            }
        }

        bool IProvider.IsCommandCancelable
        {
            get
            {
                return true;
            }
        }

        void IProvider.DeriveParameters(IDbCommand command)
        {
            TfsCommand tfsCommand = (TfsCommand)command;

            TfsDataReaderFactory.DataReaderInfo info;
            bool contains = TfsDataReaderFactory.Dictionary.TryGetValue(tfsCommand.CommandText, out info);

            if (contains)
            {
                foreach (TfsParameter parameter in info.Parameters)
                {
                    TfsParameter clone = new TfsParameter(parameter.ParameterName, parameter.Type, parameter.DbType, parameter.Direction, parameter.IsNullable,
                        parameter.DefaultValue);
                    tfsCommand.Parameters.Add(clone);
                }
            }
        }

        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
        {
            TfsParameter tfsParameter = (TfsParameter)parameter;
            return new TfsDataParameter(tfsParameter);
        }

        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
        {
            TfsParameterCollection tfsParameters = (TfsParameterCollection)parameters;
            DataTable table = new DataTable();
            DataColumnCollection columns = table.Columns;
            columns.Add("ParameterName", typeof (string));
            columns.Add("DbType", typeof (DbType));
            columns.Add("Direction", typeof (ParameterDirection));
            columns.Add("IsNullable", typeof (bool));
            columns.Add("DefaultValue");
            columns.Add("Value");
            DataRowCollection rows = table.Rows;

            foreach (TfsParameter tfsParameter in tfsParameters)
            {
                rows.Add(new object[]
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

        XmlReader IProvider.ExecuteXmlReader(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
            return new DataTable();
        }

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        Type IProvider.GetColumnType(DataColumnSchema dataColumnSchema)
        {
            DbType dbType = (DbType)dataColumnSchema.ProviderType;
            Type type;

            switch (dbType)
            {
                case DbType.DateTime:
                    type = typeof (DateTime);
                    break;

                case DbType.String:
                    type = typeof (string);
                    break;

                case DbType.Int32:
                    type = typeof (int);
                    break;

                default:
                    type = typeof (object);
                    break;
            }

            return type;
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            TfsDataReader tfsDataReader = (TfsDataReader)dataReader;
            return new TfsDataReaderHelper(tfsDataReader);
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        IObjectExplorer IProvider.ObjectExplorer
        {
            get
            {
                if (this.objectBrowser == null)
                {
                    this.objectBrowser = new TfsObjectExplorer();
                }

                return this.objectBrowser;
            }
        }

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
            var response = new GetCompletionResponse();
            string[] values = null;
            SqlStatement sqlStatement = new SqlStatement(text);
            Token[] tokens = sqlStatement.Tokens;

            if (tokens.Length > 0)
            {
                Token previousToken, currentToken;
                sqlStatement.FindToken(position, out previousToken, out currentToken);
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
                    Token token = previousToken;

                    if (token.Type == TokenType.KeyWord && string.Compare(token.Value, "exec", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        List<string> names = new List<string>();

                        foreach (string name in TfsDataReaderFactory.Dictionary.Keys)
                        {
                            names.Add(name);
                        }

                        values = names.ToArray();
                    }
                    else if (tokens.Length > 1)
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
                            TfsDataReaderFactory.DataReaderInfo info;
                            bool contains = TfsDataReaderFactory.Dictionary.TryGetValue(command.CommandText, out info);
                            if (contains)
                            {
                                TfsParameterCollection parameters = info.Parameters;
                                int parameterIndex = previousToken.Index/2;
                                if (parameterIndex < parameters.Count)
                                {
                                    TfsParameter parameter = parameters[parameterIndex];
                                    Type type = parameter.Type;
                                    if (type.IsEnum)
                                    {
                                        string[] names = Enum.GetNames(type);
                                        values = names;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            response.Items = values.Select(value => (IObjectName)new ObjectName(value)).ToList();
            return response;
        }

        void IProvider.ClearCompletionCache()
        {
            throw new NotImplementedException();
        }

        List<InfoMessage> IProvider.ToInfoMessages(Exception exception)
        {
            throw new NotImplementedException();
        }

        string IProvider.GetExceptionMessage(Exception e)
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

        #endregion
    }
}