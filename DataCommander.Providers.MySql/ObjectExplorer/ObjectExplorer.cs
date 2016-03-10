namespace DataCommander.Providers.MySql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using Foundation.Data;
    using global::MySql.Data.MySqlClient;
    using Providers;

    internal sealed class ObjectExplorer : IObjectExplorer
    {
        private string connectionString;

        public string ConnectionString => this.connectionString;

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

        bool IObjectExplorer.Sortable => false;
    }
}