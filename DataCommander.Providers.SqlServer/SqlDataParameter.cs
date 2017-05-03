namespace DataCommander.Providers.SqlServer
{
    using System.Data.SqlClient;

    internal sealed class SqlDataParameter : DataParameterBase
    {
        private readonly SqlParameter parameter;

        public SqlDataParameter(SqlParameter parameter)
            : base(parameter, parameter.Size, parameter.Precision, parameter.Scale)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(parameter != null);
#endif

            this.parameter = parameter;
        }

        protected override void SetSize(int size)
        {
            this.parameter.Size = size;
        }
    }
}