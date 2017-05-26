using System.Collections.Generic;
using System.Data;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CommandDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public string CommandText;

        /// <summary>
        /// 
        /// </summary>
        public List<object> Parameters;

        /// <summary>
        /// 
        /// </summary>
        public CommandType CommandType = CommandType.Text;

        /// <summary>
        /// 
        /// </summary>
        public int CommandTimeout;
    }
}