using System;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.SqlServer
{
    using System.Data.SqlClient;

    internal sealed class SqlDataParameter : DataParameterBase
    {
        private readonly SqlParameter parameter;

        public SqlDataParameter(SqlParameter parameter)
            : base(parameter, parameter.Size, parameter.Precision, parameter.Scale)
        {
            FoundationContract.Requires<ArgumentNullException>(parameter != null);

            this.parameter = parameter;
        }

        protected override void SetSize(int size)
        {
            parameter.Size = size;
        }
    }
}