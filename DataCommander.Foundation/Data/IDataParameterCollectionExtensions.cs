namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Text;
    using DataCommander.Foundation.Data.SqlClient;

    /// <summary>
    ///
    /// </summary>
    public static class IDataParameterCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string ToLogString( this IDataParameterCollection parameters )
        {
            Contract.Requires( parameters != null );

            var sqlParameters = parameters as SqlParameterCollection;
            string s;

            if (sqlParameters != null)
            {
                s = SqlParameterCollectionExtensions.ToLogString( sqlParameters );
            }
            else
            {
                var sb = new StringBuilder();
                bool first = true;

                foreach (IDataParameter parameter in parameters)
                {
                    if (parameter.Direction != ParameterDirection.ReturnValue)
                    {
                        object value = parameter.Value;

                        if (value != null)
                        {
                            string valueString;

                            if (value == DBNull.Value)
                            {
                                valueString = SqlNull.NullString;
                            }
                            else
                            {
                                DbType dbType = parameter.DbType;

                                switch (dbType)
                                {
                                    case DbType.DateTime:
                                        DateTime dateTime = (DateTime) value;
                                        valueString = dateTime.ToTSqlDateTime();
                                        break;

                                    case DbType.Int32:
                                        valueString = value.ToString();
                                        break;

                                    case DbType.String:
                                        valueString = "'" + value.ToString().Replace( "'", "''" ) + "'";
                                        break;

                                    default:
                                        valueString = value.ToString();
                                        break;
                                }
                            }

                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                sb.AppendLine( "," );
                            }

                            sb.AppendFormat( "  {0} = {1}", parameter.ParameterName, valueString );
                        }
                    }
                }

                s = sb.ToString();
            }

            return s;
        }
    }
}