using DataCommander.Providers2;
using Microsoft.Data.SqlClient;
using Foundation.Assertions;

namespace DataCommander.Providers.SqlServer2
{
    internal sealed class SqlDataParameter : DataParameterBase
    {
        private readonly SqlParameter _parameter;

        public SqlDataParameter(SqlParameter parameter)
            : base(parameter, parameter.Size, parameter.Precision, parameter.Scale)
        {
            Assert.IsNotNull(parameter);
            _parameter = parameter;
        }

        protected override void SetSize(int size) => _parameter.Size = size;
    }
}