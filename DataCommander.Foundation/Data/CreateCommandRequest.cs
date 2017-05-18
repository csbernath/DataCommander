namespace DataCommander.Foundation.Orm
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Data;

    /// <summary>
    /// 
    /// </summary>
    public class CreateCommandRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        public CreateCommandRequest(string commandText, IEnumerable<object> parameters, CommandType commandType, int commandTimeout)
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
        public CreateCommandRequest(string commandText, IEnumerable<object> parameters)
            : this(commandText, parameters, CommandType.Text, 0)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        public CreateCommandRequest(string commandText)
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