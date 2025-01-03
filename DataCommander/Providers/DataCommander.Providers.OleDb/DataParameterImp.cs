using DataCommander.Api;
using System.Data.OleDb;

namespace DataCommander.Providers.OleDb;

internal sealed class DataParameterImp(OleDbParameter parameter) : DataParameterBase(parameter, parameter.Size, parameter.Precision, parameter.Scale)
{
    protected override void SetSize(int size)
    {
        parameter.Size = size;
    }
}