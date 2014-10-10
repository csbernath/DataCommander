namespace DataCommander.Providers
{
    using System.Data;

    public abstract class DataParameterBase
    {
        private IDataParameter parameter;
        private int size;
        private byte precision;
        private byte scale;

        public DataParameterBase(
            IDataParameter parameter,
            int size,
            byte precision,
            byte scale)
        {
            this.parameter = parameter;
            this.size = size;
            this.precision = precision;
            this.scale = scale;
        }

        public DbType DbType
        {
            get
            {
                return parameter.DbType;
            }

            set
            {
                parameter.DbType = value;
            }
        }

        protected abstract void SetSize(int size);

        public int Size
        {
            get
            {
                return size;
            }

            set
            {
                SetSize(value);
            }
        }

        public byte Precision
        {
            get
            {
                return precision;
            }
        }

        public byte Scale
        {
            get
            {
                return scale;
            }
        }
    }
}