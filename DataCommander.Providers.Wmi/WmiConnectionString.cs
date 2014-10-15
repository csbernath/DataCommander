namespace SqlUtil.Providers.Wmi
{
    using System;
    using WAVE.Foundation.Collections;
    using WAVE.Foundation.Data;

    sealed class WmiConnectionString : ConnectionString
    {
        private WmiConnectionString(IndexedDictionary parameters)
            : base(parameters)
        {
        }

        public static WmiConnectionString FromString(string connectionString)
        {
            IndexedDictionary parameters = Parse(connectionString);
            return new WmiConnectionString(parameters);
        }

        public string DataSource
        {
            get
            {
                return (string)this["Data Source"];
            }
        }
    }
}