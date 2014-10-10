namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;
    using DataCommander.Foundation.Configuration;    

    /// <summary>
    /// This class provides a mechanism to store security sensitive data (like passwords) in a safe way.
    /// </summary>
    public static class ExternalSystem
    {
        private static Byte[] optionalEntropy = { 78, 233, 56, 11, 243, 99, 21, 165, 56, 234, 111, 9, 78, 67, 87, 96 };

        /// <summary>
        /// Gets the properties of an external system related to a specfied external system client.
        /// </summary>
        /// <param name="name">See icCore.dbo.ExternalSystem table</param>
        /// <param name="connection">Microsoft SQL Server connection</param>
        /// <returns></returns>
        public static ConfigurationAttributeCollection GetProperties( String name, IDbConnection connection )
        {
            Contract.Requires(connection != null); 

            var properties = new ConfigurationAttributeCollection { Name = name };
            DataSet dataSet = ExternalSystem_GetProperties( connection, name );
            DataTable table = dataSet.Tables[ 0 ];
            DataRow row = table.Rows[ 0 ];
            String loginame = row.Field<String>( 0 );
            String currentUser = WindowsIdentity.GetCurrent().Name;
            DataProtectionScope scope;

            if (loginame == currentUser)
            {
                scope = DataProtectionScope.CurrentUser;
            }
            else
            {
                scope = DataProtectionScope.LocalMachine;
            }

            table = dataSet.Tables[ 1 ];

            foreach (DataRow dataRow in table.Rows)
            {
                String propertyName = (String) dataRow[ 0 ];
                ExternalSystemPropertyTypes type = (ExternalSystemPropertyTypes) (Byte) dataRow[ 1 ];
                Object value = dataRow[ 2 ];
                Boolean encrypted = (type & ExternalSystemPropertyTypes.Encrypted) != 0;

                if (encrypted)
                {
                    if (value == DBNull.Value)
                    {
                        value = null;
                    }
                    else
                    {
                        byte[] bytes = (byte[]) value;
                        value = ProtectedData.Unprotect( bytes, optionalEntropy, scope );
                    }
                }

                if (type == ExternalSystemPropertyTypes.String)
                {
                    if (value == DBNull.Value)
                    {
                        value = null;
                    }
                }

                properties.Add( propertyName, value, null );
            }

            return properties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Byte[] Encrypt( DataProtectionScope scope, String text )
        {
            Byte[] bytes = Encoding.UTF8.GetBytes( text );
            Byte[] protectedBytes = ProtectedData.Protect( bytes, optionalEntropy, scope );
            return protectedBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static String Decrypt( DataProtectionScope scope, Byte[] bytes )
        {
            Byte[] unprotectedBytes = ProtectedData.Unprotect( bytes, optionalEntropy, scope );
            String text = Encoding.UTF8.GetString( unprotectedBytes );
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="bytes"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Boolean Check( DataProtectionScope scope, Byte[] bytes, String text )
        {
            String s = Decrypt( scope, bytes );
            return s == text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="systemName">varchar(64)</param>
        /// <returns></returns>
        private static DataSet ExternalSystem_GetProperties( IDbConnection connection, String systemName )
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "icCore.dbo.ExternalSystem_GetProperties";
            SqlParameterCollection parameters = (SqlParameterCollection) command.Parameters;
            parameters.Add( new SqlParameter( "@returnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, true, 0, 0, null, DataRowVersion.Current, null ) );
            parameters.Add( new SqlParameter( "@systemName", SqlDbType.VarChar, 64, ParameterDirection.Input, true, 0, 0, null, DataRowVersion.Current, null ) );
            parameters[ 1 ].Value = systemName;
            DataSet dataSet = command.ExecuteDataSet();
            return dataSet;
        }
    }
}