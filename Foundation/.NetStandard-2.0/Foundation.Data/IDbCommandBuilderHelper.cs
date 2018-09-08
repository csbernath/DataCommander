using System.Data;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDbCommandBuilderHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        void DeriveParameters(IDbCommand command);
    }
}