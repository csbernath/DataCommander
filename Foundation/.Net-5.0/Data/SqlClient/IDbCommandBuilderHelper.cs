using System.Data;

namespace Foundation.Data.SqlClient
{
    public interface IDbCommandBuilderHelper
    {
        void DeriveParameters(IDbCommand command);
    }
}