using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Foundation.Data
{
    public class CreateCommandRequest
    {
        public readonly string CommandText;
        public readonly List<object> Parameters;
        public readonly CommandType CommandType;
        public readonly int CommandTimeout;
        public readonly IDbTransaction Transaction;

        public CreateCommandRequest(string commandText, IEnumerable<object> parameters, CommandType commandType, int commandTimeout, IDbTransaction transaction)
        {
            CommandText = commandText;
            Parameters = parameters?.ToList();
            CommandType = commandType;
            CommandTimeout = commandTimeout;
            Transaction = transaction;
        }

        public CreateCommandRequest(string commandText, IEnumerable<object> parameters)
            : this(commandText, parameters, CommandType.Text, 0, null)
        {
        }

        public CreateCommandRequest(string commandText)
            : this(commandText, null)
        {
        }
    }
}