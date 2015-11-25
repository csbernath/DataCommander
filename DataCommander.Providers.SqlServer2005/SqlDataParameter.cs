namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;

    internal sealed class SqlDataParameter : DataParameterBase
    {
        private readonly SqlParameter parameter;

        public SqlDataParameter(SqlParameter parameter)
            : base(parameter, parameter.Size, parameter.Precision, parameter.Scale)
        {
            Contract.Requires<ArgumentNullException>(parameter != null);

            this.parameter = parameter;
        }

        protected override void SetSize(int size)
        {
            this.parameter.Size = size;
        }
    }
}