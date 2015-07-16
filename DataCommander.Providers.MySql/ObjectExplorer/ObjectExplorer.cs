namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.Data;
    using DataCommander.Foundation.Data;
    using DataCommander.Providers;
    using global::MySql.Data.MySqlClient;

    internal sealed class ObjectExplorer : IObjectExplorer
    {
        private string connectionString;

        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
        }

        void IObjectExplorer.SetConnection(string connectionString, System.Data.IDbConnection connection)
        {
            this.connectionString = connectionString;
        }

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            const string commandText = @"select SCHEMA_NAME
from INFORMATION_SCHEMA.SCHEMATA
order by SCHEMA_NAME";

            return MySqlClientFactory.Instance.ExecuteReader(
                this.connectionString,
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord =>
                {
                    string name = dataRecord.GetString(0);
                    return new DatabaseNode(this, name);
                });
        }

        bool IObjectExplorer.Sortable
        {
            get
            {
                return false;
            }
        }
    }
}