namespace SqlUtil.Providers.Odp
{
    using System;
    using WAVE.Foundation.Collections.Generic;
    using WAVE.Foundation.Data;

    /// <exclude/>
    [Obsolete]
    public sealed class OracleConnectionString : ConnectionString
    {
        /// <summary>
        /// 
        /// </summary>
        public OracleConnectionString()
        {
        }

        private OracleConnectionString(IndexedDictionary<string, string> parameters)
            : base(parameters)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static OracleConnectionString FromString(string connectionString)
        {
            IndexedDictionary<string,string> parameters = ConnectionString.Parse(connectionString);
            return new OracleConnectionString(parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        public string DataSource
        {
            get
            {
                return (string)this["Data Source"];
            }
            set
            {
                this["Data Source"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserId
        {
            get
            {
                return (string)this["User ID"];
            }
            set
            {
                this["User ID"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string password)
        {
            this["Password"] = password;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Enlist
        {
            get
            {
                bool enlist = true;
                object o = this["Enlist"];
                string s = o as string;

                if (s != null)
                {
                    s = s.ToLower();

                    switch (s)
                    {
                        case "true":
                            enlist = true;
                            break;

                        case "false":
                            enlist = false;
                            break;
                    }
                }

                return enlist;
            }
        }
    }
}