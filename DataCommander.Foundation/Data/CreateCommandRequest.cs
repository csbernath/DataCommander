namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class InitializeCommandRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        public InitializeCommandRequest(string commandText, IEnumerable<object> parameters, CommandType commandType, int commandTimeout)
        {
            CommandText = commandText;
            Parameters = parameters?.ToList();
            CommandType = commandType;
            CommandTimeout = commandTimeout;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        public InitializeCommandRequest(string commandText, IEnumerable<object> parameters)
            : this(commandText, parameters, CommandType.Text, 0)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        public InitializeCommandRequest(string commandText)
            : this(commandText, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public readonly string CommandText;

        /// <summary>
        /// 
        /// </summary>
        public readonly List<object> Parameters;

        /// <summary>
        /// 
        /// </summary>
        public readonly CommandType CommandType;

        /// <summary>
        /// 
        /// </summary>
        public readonly int CommandTimeout;
    }
}