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
        public static ConfigurationAttributeCollection GetProperties( string name, IDbConnection connection )
        {
            Contract.Requires(connection != null); 

            var properties = new ConfigurationAttributeCollection { Name = name };
            DataSet dataSet = ExternalSystem_GetProperties( connection, name );
            DataTable table = dataSet.Tables[ 0 ];
            DataRow row = table.Rows[ 0 ];
            string loginame = row.Field<string>( 0 );
            string currentUser = WindowsIdentity.GetCurrent().Name;
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
                string propertyName = (string) dataRow[ 0 ];
                ExternalSystemPropertyTypes type = (ExternalSystemPropertyTypes) (Byte) dataRow[ 1 ];
                object value = dataRow[ 2 ];
                bool encrypted = (type & ExternalSystemPropertyTypes.Encrypted) != 0;

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
        public static Byte[] Encrypt( DataProtectionScope scope, string text )
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
        public static string Decrypt( DataProtectionScope scope, Byte[] bytes )
        {
            Byte[] unprotectedBytes = ProtectedData.Unprotect( bytes, optionalEntropy, scope );
            string text = Encoding.UTF8.GetString( unprotectedBytes );
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="bytes"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool Check( DataProtectionScope scope, Byte[] bytes, string text )
        {
            string s = Decrypt( scope, bytes );
            return s == text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="systemName">varchar(64)</param>
        /// <returns></returns>
        private static DataSet ExternalSystem_GetProperties( IDbConnection connection, string systemName )
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