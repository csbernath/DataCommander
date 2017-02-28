namespace DataCommander.Foundation.Data.SqlClient
{
    using System.Data.SqlClient;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public static class SqlErrorCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static string ToLogString( this SqlErrorCollection errors )
        {
            string message = null;

            if (errors != null)
            {
                var sb = new StringBuilder();

                foreach (SqlError error in errors)
                {
                    var s = error.ToLogString();
                    sb.AppendLine( s );
                }

                message = sb.ToString();
            }

            return message;
        }
    }
}