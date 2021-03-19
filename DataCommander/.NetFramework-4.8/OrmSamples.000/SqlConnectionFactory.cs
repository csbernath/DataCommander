using System.Data.SqlClient;

namespace OrmSamples
{
    public static class SqlConnectionFactory
    {
        public static SqlConnection CreateSqlConnection()
        {
            var sqlConnectionString = GetSqlConnectionString();
            return new SqlConnection(sqlConnectionString);
        }

        private static string GetSqlConnectionString()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            sqlConnectionStringBuilder.DataSource = @".\SQL2017_001";
            sqlConnectionStringBuilder.InitialCatalog = "OrmSample";
            sqlConnectionStringBuilder.IntegratedSecurity = true;
            return sqlConnectionStringBuilder.ConnectionString;
        }
    }
}