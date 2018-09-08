namespace DataCommander.Providers
{
    using System.Data;

    public abstract class DataParameterBase
    {
        private readonly IDataParameter _parameter;
        private readonly int _size;

        protected DataParameterBase(IDataParameter parameter, int size, byte precision, byte scale)
        {
            _parameter = parameter;
            _size = size;
            Precision = precision;
            Scale = scale;
        }

        public DbType DbType
        {
            get => _parameter.DbType;

            set => _parameter.DbType = value;
        }

        protected abstract void SetSize(int size);

        public int Size
        {
            get => _size;

            set => SetSize(value);
        }

        public byte Precision { get; }

        public byte Scale { get; }
    }
}