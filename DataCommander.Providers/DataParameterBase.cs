namespace DataCommander.Providers
{
    using System.Data;

    public abstract class DataParameterBase
    {
        private readonly IDataParameter parameter;
        private readonly int size;

        protected DataParameterBase(
            IDataParameter parameter,
            int size,
            byte precision,
            byte scale)
        {
            this.parameter = parameter;
            this.size = size;
            this.Precision = precision;
            this.Scale = scale;
        }

        public DbType DbType
        {
            get => this.parameter.DbType;

            set => this.parameter.DbType = value;
        }

        protected abstract void SetSize(int size);

        public int Size
        {
            get => this.size;

            set => this.SetSize(value);
        }

        public byte Precision { get; }

        public byte Scale { get; }
    }
}