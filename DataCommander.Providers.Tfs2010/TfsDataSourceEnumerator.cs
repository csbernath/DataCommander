namespace DataCommander.Providers.Tfs
{
    using System.Data;
    using System.Data.Common;
    using Microsoft.TeamFoundation.Client;

    internal sealed class TfsDataSourceEnumerator : DbDataSourceEnumerator
    {
        public override DataTable GetDataSources()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ServerName", typeof(string));
            dataTable.Columns.Add("InstanceName", typeof(string));            
            string[] serverNames = RegisteredServers.GetServerNames();

            for (int i = 0; i < serverNames.Length; i++)
            {
                dataTable.Rows.Add(new object[]
                {
                    serverNames[i],
                    null
                });
            }

            return dataTable;
        }
    }
}
