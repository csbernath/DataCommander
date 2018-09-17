namespace DataCommander.Providers.OracleClient
{
    using System.Data.OracleClient;

    internal class DataParameterImp : DataParameterBase
    {
        public DataParameterImp(OracleParameter parameter)

#pragma warning disable CS0618 // Type or member is obsolete
            : base(parameter, parameter.Size, parameter.Precision, parameter.Scale)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            _parameter = parameter;
        }

        protected override void SetSize(int size)
        {
            _parameter.Size = size;
        }

        private readonly OracleParameter _parameter;
    }
}