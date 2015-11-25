namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;

    public sealed class SqlStatement
    {
        #region Private Fields

        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly string text;
        private readonly Token[] tokens;
        private readonly IDictionary<string, string> tables;
        private readonly Table[] allTables;

        #endregion

        public SqlStatement(string text)
        {
            this.text = text;
            this.tokens = Tokenize(text);
            Table[] allTables;
            this.tables = FindTables(this.tokens, out allTables);
            this.allTables = allTables;
            foreach (string value in this.tables.Values)
            {
                log.Write(LogLevel.Trace, value);
            }
        }

        #region Public Properties

        public IDictionary<string, string> Tables
        {
            get
            {
                return this.tables;
            }
        }

        public Token[] Tokens
        {
            get
            {
                return this.tokens;
            }
        }

        #endregion

        #region Public Methods

        public IDbCommand CreateCommand(
            IProvider provider,
            ConnectionBase connection,
            CommandType commandType,
            int commandTimeout)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandTimeout = commandTimeout;
            CommandType commandType2 = commandType;

            if (this.tokens.Length > 0)
            {
                Token firstToken = this.tokens[0];
                int startTokenIndex = 0;
                bool isVBScript = false;
                string query2 = null;

                if (firstToken.Type == TokenType.KeyWord)
                {
                    string keyWord = firstToken.Value.ToLower();
                    query2 = this.text;

                    switch (keyWord)
                    {
                        case "exec":
                            commandType2 = CommandType.StoredProcedure;
                            startTokenIndex = 1;
                            break;

                        case "load":
                            commandType2 = CommandType.Text;
                            break;

                        case "main":
                            commandType2 = CommandType.StoredProcedure;
                            isVBScript = true;
                            break;

                        case "select":
                            commandType2 = CommandType.Text;
                            break;

                        default:
                            break;
                    }
                }

                command.CommandType = commandType2;

                switch (commandType2)
                {
                    case CommandType.Text:
                        command.CommandText = query2;
                        break;

                    default:
                        if (isVBScript)
                        {
                            //string commandText = query.Substring(firstLine.Length);
                            //command.CommandText = commandText;
                        }
                        else
                        {
                            command.CommandText = this.tokens[startTokenIndex].Value;
                        }

                        startTokenIndex++;
                        provider.DeriveParameters(command);

                        int i = startTokenIndex;
                        List<Token> tokenList = new List<Token>();
                        List<Parameter> parameters = new List<Parameter>();
                        while (i < this.tokens.Length)
                        {
                            var token = this.tokens[i];
                            if (token.Type == TokenType.OperatorOrPunctuator && token.Value == ",")
                            {
                                parameters.Add(ToParameter(tokenList));
                                tokenList.Clear();
                            }
                            else
                            {
                                tokenList.Add(token);
                            }
                            i++;
                        }
                        if (tokenList.Count > 0)
                        {
                            parameters.Add(ToParameter(tokenList));
                        }

                        List<IDataParameter> defaultValues = new List<IDataParameter>();
                        foreach (IDataParameter parameter in command.Parameters)
                        {
                            switch (parameter.Direction)
                            {
                                case ParameterDirection.Input:
                                case ParameterDirection.InputOutput:
                                    DataParameterBase dataParameter = provider.GetDataParameter(parameter);
                                    var first = parameters.FirstOrDefault(p=>string.Compare(p.Name, parameter.ParameterName, StringComparison.InvariantCultureIgnoreCase) == 0);
                                    if (first == null)
                                    {
                                        first = parameters.FirstOrDefault(p => p.Name == null);
                                        if (first != null)
                                        {
                                            parameters.Remove(first);
                                        }
                                    }
                                    if (first != null)
                                    {
                                        object value = this.GetParameterValue(dataParameter, first.Value);
                                        if (value != null)
                                        {
                                            parameter.Value = value;
                                        }
                                        else
                                        {
                                            defaultValues.Add(parameter);
                                        }
                                    }
                                    break;
                            }
                        }
                        foreach (var parameter in defaultValues)
                        {
                            command.Parameters.Remove(parameter);
                        }
                        break;
                }
            }
            else
            {
                command.CommandText = this.text;
            }

            return command;
        }

        public SqlObject FindSqlObject(int index)
        {
            SqlObject sqlObject = null;
            if (index >= 1)
            {
                Token prev = this.tokens[index - 1];

                if (prev.Type == TokenType.KeyWord)
                {
                    string value = prev.Value.ToLower();
                    string name = null;

                    if (index < this.tokens.Length)
                    {
                        Token token = this.tokens[index];
                        name = token.Value;
                    }

                    switch (value)
                    {
                        case "use":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Database, null);
                            break;

                        case "delete":
                        case "insert":
                        case "into":
                        case "update":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Table | SqlObjectTypes.View, name);
                            break;

                        case "from":
                        case "join":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Table | SqlObjectTypes.View | SqlObjectTypes.Function, name);
                            break;

                        case "table":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Table, name);
                            break;

                        case "exec":
                        case "proc":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Procedure, name);
                            break;

                        case "function":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Function, null);
                            break;

                        case "trigger":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Trigger, null);
                            break;

                        case "view":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.View, null);
                            break;

                        case "index":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Index, null);
                            break;

                        case "where":
                            index = this.allTables.LastIndexOf(t => t.Index < index - 1);
                            if (index >= 0)
                            {
                                Table table = this.allTables[index];
                                sqlObject = new SqlObject(table.Name, null, SqlObjectTypes.Column, name);
                            }
                            break;

                        default:
                            break;
                    }

                    if (sqlObject == null && index >= 0 && index < this.tokens.Length)
                    {
                        Token token = this.tokens[index];
                        sqlObject = this.GetSqlObject(token.Value);
                    }
                }
                else if (prev.Type == TokenType.OperatorOrPunctuator)
                {
                    Token tokenAfterOperator = index < this.tokens.Length ? this.tokens[index] : null;
                    if (tokenAfterOperator != null && tokenAfterOperator.Value.Contains('.'))
                    {
                        sqlObject = this.GetSqlObject(tokenAfterOperator.Value);
                    }
                    else if (prev.Value == "=" && index >= 2)
                    {
                        Token tokenBeforeOperator = this.tokens[index - 2];
                        sqlObject = new SqlObject(tokenBeforeOperator.Value, null, SqlObjectTypes.Value, null);
                    }
                }
            }

            return sqlObject;
        }

        public SqlObject FindSqlObject(Token previousToken, Token currentToken)
        {
            SqlObject sqlObject = null;
            if (previousToken != null)
            {
                if (previousToken.Type == TokenType.KeyWord)
                {
                    string value = previousToken.Value.ToLower();
                    string name = currentToken != null ? currentToken.Value : null;

                    switch (value)
                    {
                        case "use":
                        case "database":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Database, value);
                            break;

                        case "delete":
                        case "insert":
                        case "update":
                        case "into":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Table | SqlObjectTypes.View, name);
                            break;

                        case "from":
                        case "join":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Table | SqlObjectTypes.View | SqlObjectTypes.Function, name);
                            break;

                        case "table":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Table, name);
                            break;

                        case "exec":
                        case "proc":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Procedure, name);
                            break;

                        case "function":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Function, null);
                            break;

                        case "trigger":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Trigger, null);
                            break;

                        case "view":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.View, null);
                            break;

                        case "index":
                            sqlObject = new SqlObject(null, null, SqlObjectTypes.Index, null);
                            break;

                        case "where":
                            //
                            if (currentToken != null)
                            {
                                sqlObject = this.GetSqlObject( currentToken.Value );
                            }
                            if (sqlObject == null)
                            {
                                int index = this.allTables.LastIndexOf( t => t.Index < previousToken.Index - 1 );
                                if (index >= 0)
                                {
                                    Table table = this.allTables[ index ];
                                    sqlObject = new SqlObject(table.Name, null, SqlObjectTypes.Column, name);
                                }
                            }
                            break;

                        default:
                            break;
                    }

                    if (sqlObject == null && currentToken != null)
                    {
                        sqlObject = this.GetSqlObject( currentToken.Value );
                    }
                }
                else if (previousToken.Type == TokenType.OperatorOrPunctuator)
                {
                    if (currentToken != null && currentToken.Value.Contains( '.' ))
                    {
                        sqlObject = this.GetSqlObject( currentToken.Value );
                    }
                    else if (previousToken.Value == "=" && previousToken.Index > 0)
                    {
                        var tokenBeforeOperator = this.tokens[ previousToken.Index - 1 ];
                        sqlObject = new SqlObject(tokenBeforeOperator.Value, null, SqlObjectTypes.Value, null);
                    }
                }
            }

            return sqlObject;
        }

        public string FindTableName()
        {
            string tableName = null;
            int i;

            for (i = 0; i < this.tokens.Length; i++)
            {
                Token token = this.tokens[i];

                if (token.Type == TokenType.KeyWord)
                {
                    if (string.Compare(token.Value, "from", true) == 0)
                    {
                        break;
                    }
                }
            }

            i++;

            if (i < this.tokens.Length)
            {
                Token token = this.tokens[i];

                if (token.Type == TokenType.KeyWord)
                {
                    tableName = token.Value;
                }
            }

            return tableName;
        }

        public int FindToken(int position)
        {
            int index = -1;
            int last = this.tokens.Length - 1;
            for (int i = 0; i <= last; i++)
            {
                var token = this.tokens[i];

                if (position < token.StartPosition)
                {
                    index = i;
                    break;
                }
                else if (token.StartPosition <= position && position <= token.EndPosition + 1)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                var lastToken = this.tokens[ last ];
                if (lastToken.EndPosition < position)
                {
                    index = last + 1;
                }
            }
            return index;
        }

        public void FindToken( int position, out Token previousToken, out Token currentToken)
        {
            previousToken = null;
            currentToken = null;

            for (int i = 0; i < this.tokens.Length; i++)
            {
                var token = this.tokens[ i ];
                if (token.EndPosition + 1 < position)
                {
                    previousToken = token;
                }
                else if (token.StartPosition <= position && position <= token.EndPosition + 1)
                {
                    currentToken = token;
                    break;
                }
                else
                {
                    break;
                }
            }
        }

        #endregion

        #region Private Methods

        private static Token[] Tokenize(string text)
        {
            var tokenList = new List<Token>();
            var iterator = new TokenIterator(text);

            while (true)
            {
                Token token = iterator.Next();

                if (token == null)
                {
                    break;
                }

                tokenList.Add(token);
            }

            Token[] tokenArray = new Token[tokenList.Count];
            tokenList.CopyTo(tokenArray);
            return tokenArray;
        }

        private object GetParameterValue(
            DataParameterBase dataParameter,
            object value)
        {
            object value2;

            if (value != null)
            {
                if (value == DBNull.Value)
                {
                    value2 = DBNull.Value;
                }
                else
                {
                    switch (dataParameter.DbType)
                    {
                        case DbType.Boolean:
                            string valueStr = (string)value;
                            double valueDbl;
                            bool ok = double.TryParse(valueStr, NumberStyles.Any, null, out valueDbl);

                            if (ok)
                            {
                                value2 = Convert.ToBoolean(valueDbl);
                            }
                            else
                            {
                                value2 = Convert.ToBoolean(value);
                            }

                            break;

                        case DbType.Int16:
                            value2 = Convert.ToInt32(value);
                            break;

                        case DbType.Int32:
                            value2 = Convert.ToInt32(value);
                            break;

                        case DbType.Decimal:
                        case DbType.VarNumeric:
                            valueStr = value.ToString();
                            DecimalString decimalString = new DecimalString(value.ToString());

                            if (dataParameter.Precision != 0 && decimalString.Precision > dataParameter.Precision)
                            {
                                throw new Exception("Invalid precision");
                            }
                            else if (dataParameter.Scale != 0 && decimalString.Scale > dataParameter.Scale)
                            {
                                throw new Exception("Invalid scale");
                            }
                            else
                            {
                                IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                                value2 = Convert.ToDecimal(valueStr, formatProvider);
                            }

                            break;

                        case DbType.Double:
                            value2 = Convert.ToDouble(value);
                            break;

                        case DbType.String:
                        case DbType.StringFixedLength:
                        case DbType.AnsiString:
                        case DbType.AnsiStringFixedLength:
                            valueStr = Convert.ToString(value);

                            if (dataParameter.Size > 0 && valueStr.Length > dataParameter.Size)
                            {
                                throw new Exception("Length exceeds size of parameter");
                            }
                            else
                            {
                                value2 = valueStr;
                            }

                            break;

                        case DbType.DateTime:
                            try
                            {
                                value2 = Convert.ToDateTime(value);
                            }
                            catch
                            {
                                string[] formats = new string[]
								{
									"yyyyMMdd",
									"yyyyMMdd HH:mm:ss"
								};

                                IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                                value2 = DateTime.ParseExact(value.ToString(), formats, formatProvider, DateTimeStyles.None);
                            }

                            break;

                        default:
                            value2 = value;
                            break;
                    }
                }
            }
            else
            {
                value2 = null;
            }

            return value2;
        }

        private void SetParameterValues(
            IProvider provider,
            IDataParameterCollection parameters,
            object[] values)
        {
            if (values.Length > parameters.Count)
            {
                throw new Exception("Too many arguments");
            }

            int j = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                IDataParameter parameter = (IDataParameter)parameters[i];
                DataParameterBase dataParameter = provider.GetDataParameter(parameter);

                if (j < values.Length)
                {
                    switch (parameter.Direction)
                    {
                        case ParameterDirection.Input:
                        case ParameterDirection.InputOutput:
                            try
                            {
                                object value = this.GetParameterValue(dataParameter, values[j]);
                                parameter.Value = value;
                                j++;
                            }
                            catch (Exception e)
                            {
                                string message = $"Invalid parameter value: {values[j]}\r\nIndex: {j}.\r\nMessage: {e.Message}";
                                throw new ArgumentException(message, parameter.ParameterName, e);
                            }

                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    switch (parameter.Direction)
                    {
                        case ParameterDirection.Input:
                        case ParameterDirection.InputOutput:
                            parameter.Value = DBNull.Value;
                            break;

                        default:
                            break;
                    }
                }

                if (parameter.Direction != ParameterDirection.Input)
                {
                    int size = dataParameter.Size;

                    if (size == 0)
                    {
                        switch (parameter.DbType)
                        {
                            case DbType.AnsiString:
                            case DbType.AnsiStringFixedLength:
                            case DbType.String:
                            case DbType.StringFixedLength:
                                dataParameter.Size = 255;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        private static object ToParameterValue(Token token)
        {
            string tokenValue = token.Value;
            object value;
            if (token.Type == TokenType.KeyWord && string.Compare(tokenValue, "null", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                value = DBNull.Value;
            }
            else if (token.Type == TokenType.KeyWord && string.Compare(tokenValue, "default", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                value = null;
            }
            else
            {
                value = tokenValue;
            }
            return value;
        }

        private static Parameter ToParameter(List<Token> tokens)
        {
            string name;
            object value;
            int count = tokens.Count;
            if (count == 0)
            {
                name = null;
                value = null;
            }
            else if (count == 1)
            {
                name = null;
                value = ToParameterValue(tokens[0]);
            }
            else if (count == 3)
            {
                name = tokens[0].Value;
                value = ToParameterValue(tokens[2]);
            }
            else
            {
                throw new NotImplementedException();
            }
            return new Parameter(name, value);
        }

        private static IDictionary<string, string> FindTables( Token[] tokens, out Table[] allTables )
        {
            Dictionary<string, string> tables = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            List<Table> tableList = new List<Table>();

            for (int i = 0; i < tokens.Length; i++)
            {
                Token token = tokens[i];
                string value = token.Value;

                if (token.Type == TokenType.KeyWord && value != null && i < tokens.Length - 1)
                {
                    switch (value.ToLower())
                    {
                        case "from":
                        case "join":
                            token = tokens[i + 1];

                            if (token.Type == TokenType.KeyWord)
                            {
                                string tableName = token.Value;

                                if (i < tokens.Length - 2)
                                {
                                    token = tokens[i + 2];

                                    if (token.Type == TokenType.KeyWord)
                                    {
                                        string alias = token.Value;
                                        tableList.Add(new Table(i, tableName, alias));
                                        if (!tables.ContainsKey(alias))
                                        {
                                            tables.Add(alias, tableName);
                                        }
                                    }
                                }
                            }

                            break;
                    }
                }
            }

            allTables = tableList.ToArray();
            return tables;
        }

        private SqlObject GetSqlObject(string value)
        {
            SqlObject sqlObject = null;
            string[] items = value.Split('.');
            if (items.Length > 1)
            {
                string alias = items[0];
                string tableName;
                bool contains = this.tables.TryGetValue(alias, out tableName);

                if (contains)
                {
                    string name = items[1];
                    sqlObject = new SqlObject(tableName, alias, SqlObjectTypes.Column, name);
                }
                else
                {
                    sqlObject = new SqlObject(null, null, SqlObjectTypes.Function, value);
                }
            }
            return sqlObject;
        }

        #endregion

        #region Private Classes

        private sealed class Table
        {
            private readonly int index;
            private readonly string name;
            private string alias;

            public Table( int index, string name, string alias )
            {
                this.index = index;
                this.name = name;
                this.alias = alias;
            }

            public int Index
            {
                get
                {
                    return this.index;
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }
        }

        private sealed class Parameter
        {
            private readonly string name;
            private readonly object value;

            public Parameter(string name, object value)
            {
                this.name = name;
                this.value = value;
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public object Value
            {
                get
                {
                    return this.value;
                }
            }
        }

        #endregion
    }
}