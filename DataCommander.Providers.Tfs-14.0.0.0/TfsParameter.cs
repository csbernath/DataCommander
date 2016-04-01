namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;

    internal sealed class TfsParameter : IDataParameter
    {
        public TfsParameter(string name, Type type, DbType dbType, ParameterDirection direction, bool isNullable, object defaultValue)
        {
            this.ParameterName = name;
            this.Type = type;
            this.DbType = dbType;
            this.Direction = direction;
            this.IsNullable = isNullable;
            this.DefaultValue = defaultValue;
        }

        public Type Type { get; }

        public object DefaultValue { get; }

        #region IDataParameter Members

        public DbType DbType { get; set; }

        public ParameterDirection Direction { get; set; }

        public bool IsNullable { get; }

        public string ParameterName { get; set; }

        string IDataParameter.SourceColumn
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        DataRowVersion IDataParameter.SourceVersion
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public object Value { get; set; }

        #endregion
    }
}
