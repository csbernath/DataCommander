namespace DataCommander.Providers.Tfs
{
    using System.Data;
    using System.Data.Common;
    using Microsoft.TeamFoundation.Client;

    internal sealed class TfsDataSourceEnumerator : DbDataSourceEnumerator
    {
        public override DataTable GetDataSources()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("ServerName", typeof(string));
            dataTable.Columns.Add("InstanceName", typeof(string));
            var projectCollections = RegisteredTfsConnections.GetProjectCollections();

            foreach(var projectCollection in projectCollections)
            {
                dataTable.Rows.Add(new object[]
                {
                    projectCollection.Uri,
                    null
                });
            }

            return dataTable;
        }
    }
}
