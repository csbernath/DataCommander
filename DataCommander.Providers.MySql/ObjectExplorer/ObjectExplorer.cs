namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataCommander.Providers;
    using Foundation.Data;
    using Foundation.Linq;
    using global::MySql.Data.MySqlClient;
    using DataCommander.Foundation.Data;

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

            using (var connection = new MySqlConnection(this.connectionString))
            {
                connection.Open();
                using (var context = connection.ExecuteReader(null, commandText, CommandType.Text, 0, CommandBehavior.Default))
                {
                    var dataReader = context.DataReader;
                    while (dataReader.Read())
                    {
                        string name = dataReader.GetString(0);
                        yield return new DatabaseNode(this, name);
                    }
                }
            }
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