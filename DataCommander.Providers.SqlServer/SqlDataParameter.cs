using Foundation.Diagnostics.Assertions;

namespace DataCommander.Providers.SqlServer
{
    using System.Data.SqlClient;

    internal sealed class SqlDataParameter : DataParameterBase
    {
        private readonly SqlParameter parameter;

        public SqlDataParameter(SqlParameter parameter)
            : base(parameter, parameter.Size, parameter.Precision, parameter.Scale)
        {
            Assert.IsNotNull(parameter);
            this.parameter = parameter;
        }

        protected override void SetSize(int size)
        {
            parameter.Size = size;
        }
    }
}