using Foundation.Data;

namespace DataCommander.Providers.MySql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using global::MySql.Data.MySqlClient;
    using Providers;

    internal sealed class ObjectExplorer : IObjectExplorer
    {
        public string ConnectionString { get; private set; }

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            ConnectionString = connectionString;
        }

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            const string commandText = @"select SCHEMA_NAME
from INFORMATION_SCHEMA.SCHEMATA
order by SCHEMA_NAME";

            return MySqlClientFactory.Instance.ExecuteReader(
                ConnectionString,
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    return new DatabaseNode(this, name);
                });
        }

        bool IObjectExplorer.Sortable => false;
    }
}