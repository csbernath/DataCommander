namespace DataCommander.Providers
{
    using System;
    using System.Data;
    using DataCommander.Foundation.Data;

    public static class Extensions
    {
        public static DataSet ExecuteDataSet( this IDbConnection connection, String commandText )
        {
            return connection.ExecuteDataSet( null, commandText, CommandType.Text, 0 );
        }

        public static DataTable ExecuteDataTable( this IDbConnection connection, String commandText )
        {
            return connection.ExecuteDataTable( null, commandText, CommandType.Text, 0 );
        }

        public static IDataReader ExecuteReader( this IDbConnection connection, String commandText )
        {
            return connection.ExecuteReader( null, commandText, CommandType.Text, 0, CommandBehavior.Default ).DataReader;
        }

        public static Object ExecuteScalar( this IDbConnection connection, String commandText )
        {
            return connection.ExecuteScalar( null, commandText, CommandType.Text, 0 );
        }
    }
}