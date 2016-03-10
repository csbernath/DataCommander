namespace DataCommander.Providers.OleDb
{
    using System.Data.OleDb;

    internal sealed class DataParameterImp : DataParameterBase
    {
        public DataParameterImp(OleDbParameter parameter)
            : base(parameter, parameter.Size, parameter.Precision, parameter.Scale)
        {
            this.parameter = parameter;
        }

        protected override void SetSize(int size)
        {
            this.parameter.Size = size;
        }

        private readonly OleDbParameter parameter;
    }
}