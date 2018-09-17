using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp
{
    internal sealed class DataParameterImp : DataParameterBase
    {
        public DataParameterImp( OracleParameter parameter )
            : base( parameter, parameter.Size, parameter.Precision, parameter.Scale )
        {
            this.parameter = parameter;
        }

        protected override void SetSize( int size )
        {
            parameter.Size = size;
        }

        readonly OracleParameter parameter;
    }
}