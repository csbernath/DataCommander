using DataCommander.Api;
using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp;

internal sealed class DataParameterImp : DataParameterBase
{
    public DataParameterImp(OracleParameter parameter)
        : base(parameter, parameter.Size, parameter.Precision, parameter.Scale) => this._parameter = parameter;

    protected override void SetSize(int size) => _parameter.Size = size;

    private readonly OracleParameter _parameter;
}