using DataCommander.Providers2;
using System.Data.OleDb;

namespace DataCommander.Providers.OleDb
{
    internal sealed class DataParameterImp : DataParameterBase
    {
        public DataParameterImp(OleDbParameter parameter)
            : base(parameter, parameter.Size, parameter.Precision, parameter.Scale)
        {
            this.parameter = parameter;
        }

        protected override void SetSize(int size)
        {
            parameter.Size = size;
        }

        private readonly OleDbParameter parameter;
    }
}