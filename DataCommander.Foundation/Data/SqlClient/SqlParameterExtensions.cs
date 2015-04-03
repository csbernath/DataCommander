namespace DataCommander.Foundation.Data.SqlClient
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public static class SqlParameterExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GetDataTypeName( this SqlParameter parameter )
        {
            var sb = new StringBuilder();
            sb.Append( parameter.SqlDbType.ToString().ToLowerInvariant() );

            switch (parameter.SqlDbType)
            {
                case SqlDbType.Decimal:
                    if (parameter.Scale == 0)
                    {
                        sb.AppendFormat( "({0})", parameter.Precision );
                    }
                    else
                    {
                        sb.AppendFormat( "({0},{1})", parameter.Precision, parameter.Scale );
                    }
                    break;

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                    int size = parameter.Size;
                    string sizeString;

                    if (size == -1 || size == int.MaxValue)
                    {
                        sizeString = "max";
                    }
                    else
                    {
                        sizeString = size.ToString();
                    }

                    sb.AppendFormat( "({0})", sizeString );
                    break;
            }

            return sb.ToString();
        }
    }
}