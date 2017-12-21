using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Foundation.Data.SqlClient;

namespace Foundation.Data
{
    /// <summary>
    ///
    /// </summary>
    public static class IDataParameterCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataParameterCollection"></param>
        /// <param name="parameters"></param>
        public static void AddRange(this IDataParameterCollection dataParameterCollection, IEnumerable<object> parameters)
        {
            foreach (var parameter in parameters)
                dataParameterCollection.Add(parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<object> ToObjectList(this IEnumerable<IDataParameter> parameters)
        {
            return new List<object>(parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string ToLogString( this IDataParameterCollection parameters )
        {
#if CONTRACTS_FULL
            FoundationContract.Requires( parameters != null );
#endif

            var sqlParameters = parameters as SqlParameterCollection;
            string s;

            if (sqlParameters != null)
            {
                s = SqlParameterCollectionExtensions.ToLogString( sqlParameters );
            }
            else
            {
                var sb = new StringBuilder();
                var first = true;

                foreach (IDataParameter parameter in parameters)
                {
                    if (parameter.Direction != ParameterDirection.ReturnValue)
                    {
                        var value = parameter.Value;

                        if (value != null)
                        {
                            string valueString;

                            if (value == DBNull.Value)
                            {
                                valueString = SqlNull.NullString;
                            }
                            else
                            {
                                var dbType = parameter.DbType;

                                switch (dbType)
                                {
                                    case DbType.DateTime:
                                        var dateTime = (DateTime) value;
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