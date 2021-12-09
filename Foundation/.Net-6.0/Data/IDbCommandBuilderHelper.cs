using System.Data;

namespace Foundation.Data;

public interface IDbCommandBuilderHelper
{
    void DeriveParameters(IDbCommand command);
}