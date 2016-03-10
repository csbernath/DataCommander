namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;

    internal sealed class TfsParameter : IDataParameter
    {
        private string name;
        private readonly Type type;
        private DbType dbType;
        private ParameterDirection direction;
        private readonly bool isNullable;
        private readonly object defaultValue;
        private object value;

        public TfsParameter(string name, Type type, DbType dbType, ParameterDirection direction, bool isNullable, object defaultValue)
        {
            this.name = name;
            this.type = type;
            this.dbType = dbType;
            this.direction = direction;
            this.isNullable = isNullable;
            this.defaultValue = defaultValue;
        }

        public Type Type => this.type;

        public object DefaultValue => this.defaultValue;

        #region IDataParameter Members

        public DbType DbType
        {
            get
            {
                return this.dbType;
            }

            set
            {
                this.dbType = value;
            }
        }

        public ParameterDirection Direction
        {
            get
            {
                return this.direction;
            }

            set
            {
                this.direction = value;
            }
        }

        public bool IsNullable => this.isNullable;

        public string ParameterName
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

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

        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
            }
        }

        #endregion
    }
}
