using DataCommander.Providers2;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsDataParameter : DataParameterBase
    {
        public TfsDataParameter(TfsParameter parameter)
            : base(parameter, 0, 0, 0)
        {
        }

        protected override void SetSize(int size)
        {
        }
    }
}
