using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Foundation.Assertions;
using Foundation.Configuration;

namespace Foundation.Data.SqlClient
{
    /// <summary>
    /// This class provides a mechanism to store security sensitive data (like passwords) in a safe way.
    /// </summary>
    public static class ExternalSystem
    {
        private static readonly byte[] OptionalEntropy = {78, 233, 56, 11, 243, 99, 21, 165, 56, 234, 111, 9, 78, 67, 87, 96};

        /// <summary>
        /// Gets the properties of an external system related to a specfied external system client.
        /// </summary>
        /// <param name="name">See icCore.dbo.ExternalSystem table</param>
        /// <param name="connection">Microsoft SQL Server connection</param>
        /// <returns></returns>
        public static ConfigurationAttributeCollection GetProperties(string name, IDbConnection connection)
        {
            Assert.IsNotNull(connection);

            var properties = new ConfigurationAttributeCollection {Name = name};
            var dataSet = ExternalSystem_GetProperties(connection, name);
            var table = dataSet.Tables[0];
            var row = table.Rows[0];
            var loginame = row.GetReferenceField<string>(0);
            var currentUser = WindowsIdentity.GetCurrent().Name;
            DataProtectionScope scope;

            scope = loginame == currentUser ? DataProtectionScope.CurrentUser : DataProtectionScope.LocalMachine;

            table = dataSet.Tables[1];

            foreach (DataRow dataRow in table.Rows)
            {
                var propertyName = (string) dataRow[0];
                var type = (ExternalSystemPropertyTypes) (byte) dataRow[1];
                var value = dataRow[2];
                var encrypted = (type & ExternalSystemPropertyTypes.Encrypted) != 0;

                if (encrypted)
                {
                    if (value == DBNull.Value)
                        value = null;
                    else
                    {
                        var bytes = (byte[]) value;
                        value = ProtectedData.Unprotect(bytes, OptionalEntropy, scope);
                    }
                }

                if (type == ExternalSystemPropertyTypes.String)
                {
                    if (value == DBNull.Value)
                        value = null;
                }

                properties.Add(propertyName, value, null);
            }

            return properties;
        }

        public static byte[] Encrypt(DataProtectionScope scope, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var protectedBytes = ProtectedData.Protect(bytes, OptionalEntropy, scope);
            return protectedBytes;
        }

        public static string Decrypt(DataProtectionScope scope, byte[] bytes)
        {
            var unprotectedBytes = ProtectedData.Unprotect(bytes, OptionalEntropy, scope);
            var text = Encoding.UTF8.GetString(unprotectedBytes);
            return text;
        }

        public static bool Check(DataProtectionScope scope, byte[] bytes, string text)
        {
            var decrypted = Decrypt(scope, bytes);
            return decrypted == text;
        }

        private static DataSet ExternalSystem_GetProperties(IDbConnection connection, string systemName)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "icCore.dbo.ExternalSystem_GetProperties";
            var parameters = (SqlParameterCollection) command.Parameters;
            parameters.Add(new SqlParameter
            {
                ParameterName = "@returnValue",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.ReturnValue
            });
            parameters.Add(new SqlParameter
            {
                ParameterName = "@systemName",
                SqlDbType = SqlDbType.VarChar,
                Size = 64,
                Direction = ParameterDirection.Input,
                Value = systemName
            });
            var dataSet = command.ExecuteDataSet(CancellationToken.None);
            return dataSet;
        }
    }
}