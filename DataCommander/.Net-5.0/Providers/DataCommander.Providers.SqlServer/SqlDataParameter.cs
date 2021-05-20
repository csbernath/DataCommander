using DataCommander.Providers2;
using Foundation.Assertions;
using System.Data.SqlClient;

namespace DataCommander.Providers.SqlServer
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