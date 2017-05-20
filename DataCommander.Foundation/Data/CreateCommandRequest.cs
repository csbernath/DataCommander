namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class CreateCommandRequest
    {
        public CreateCommandRequest(string commandText, IEnumerable<object> parameters, CommandType commandType, int commandTimeout)
        {
            CommandText = commandText;
            Parameters = parameters?.ToList();
            CommandType = commandType;
            CommandTimeout = commandTimeout;
        }

        public CreateCommandRequest(string commandText, IEnumerable<object> parameters)
            : this(commandText, parameters, CommandType.Text, 0)
        {
        }

        public CreateCommandRequest(string commandText)
            : this(commandText, null)
        {
        }

        public readonly string CommandText;
        public readonly List<object> Parameters;
        public readonly CommandType CommandType;
        public readonly int CommandTimeout;
    }
}