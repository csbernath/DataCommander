namespace DataCommander.Foundation.Orm
{
    using System.Data.SqlClient;

    /// <summary>
    /// 
    /// </summary>
    public static class SqlServerOrm
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static OrmContext CreateContext(string connectionString)
        {
            return new OrmContext(() => null, () => new SqlConnection(connectionString));
        }
    }
}