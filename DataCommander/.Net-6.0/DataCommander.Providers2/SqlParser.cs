using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using DataCommander.Providers2.Connection;
using DataCommander.Providers2.FieldNamespace;
using DataCommander.Providers2.Query;
using Foundation.Linq;
using Foundation.Log;

namespace DataCommander.Providers2;

public sealed class SqlParser
{
    #region Private Fields

    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private readonly string _text;
    private readonly Table[] _allTables;

    #endregion

    public SqlParser(string text)
    {
        _text = text;
        Tokens = Tokenize(text);
        Tables = FindTables(Tokens, out var allTables);
        _allTables = allTables;

        foreach (var value in Tables.Values)
            Log.Write(LogLevel.Trace, value);
    }

    #region Public Properties

    public IDictionary<string, string> Tables { get; }
    public List<Token> Tokens { get; }

    #endregion

    #region Public Methods

    public IDbCommand CreateCommand(IProvider provider, ConnectionBase connection, CommandType commandType, int commandTimeout)
    {
        var command = connection.CreateCommand();
        command.CommandType = commandType;
        command.CommandTimeout = commandTimeout;
        var commandType2 = commandType;

        if (Tokens.Count > 0)
        {
            var firstToken = Tokens[0];
            var startTokenIndex = 0;
            var isVbScript = false;

            if (firstToken.Type == TokenType.KeyWord)
            {
                var keyWord = firstToken.Value.ToLower();

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
                        isVbScript = true;
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
                    command.CommandText = _text;
                    break;

                default:
                    if (isVbScript)
                    {
                        //string commandText = query.Substring(firstLine.Length);
                        //command.CommandText = commandText;
                    }
                    else
                        command.CommandText = Tokens[startTokenIndex].Value;

                    startTokenIndex++;
                    provider.DeriveParameters(command);

                    var tokens = Tokens.GetRange(startTokenIndex, Tokens.Count - startTokenIndex);
                    var parameterTokens = tokens.Split(t => t.Type == TokenType.OperatorOrPunctuator && t.Value == ",").ToList();
                    var parameters = parameterTokens.Select(t => ToParameter(t.ToList())).ToList();

                    var defaultValues = new List<IDataParameter>();
                    foreach (IDataParameter parameter in command.Parameters)
                    {
                        switch (parameter.Direction)
                        {
                            case ParameterDirection.Input:
                            case ParameterDirection.InputOutput:
                                var dataParameter = provider.GetDataParameter(parameter);
                                var first = parameters.FirstOrDefault(
                                    p => string.Compare(p.Name, parameter.ParameterName,
                                        StringComparison.InvariantCultureIgnoreCase) == 0);
                                if (first == null)
                                {
                                    first = parameters.FirstOrDefault(p => p.Name == null);
                                    if (first != null)
                                        parameters.Remove(first);
                                }

                                if (first != null)
                                {
                                    var value = GetParameterValue(dataParameter, first.Value);
                                    if (value != null)
                                        parameter.Value = value;
                                    else
                                        defaultValues.Add(parameter);
                                }

                                break;
                        }
                    }

                    foreach (var parameter in defaultValues)
                        command.Parameters.Remove(parameter);
                    break;
            }
        }
        else
            command.CommandText = _text;

        return command;
    }

    public SqlObject FindSqlObject(int index)
    {
        SqlObject sqlObject = null;
        if (index >= 1)
        {
            var prev = Tokens[index - 1];

            if (prev.Type == TokenType.KeyWord)
            {
                var value = prev.Value.ToLower();
                string name = null;

                if (index < Tokens.Count)
                {
                    var token = Tokens[index];
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
                        sqlObject = new SqlObject(null, null,
                            SqlObjectTypes.Table | SqlObjectTypes.View | SqlObjectTypes.Function, name);
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
                        index = _allTables.LastIndexOf(t => t.Index < index - 1);
                        if (index >= 0)
                        {
                            var table = _allTables[index];
                            sqlObject = new SqlObject(table.Name, null, SqlObjectTypes.Column, name);
                        }

                        break;

                    default:
                        break;
                }

                if (sqlObject == null && index >= 0 && index < Tokens.Count)
                {
                    var token = Tokens[index];
                    sqlObject = GetSqlObject(token.Value);
                }
            }
            else if (prev.Type == TokenType.OperatorOrPunctuator)
            {
                var tokenAfterOperator = index < Tokens.Count
                    ? Tokens[index]
                    : null;
                if (tokenAfterOperator != null && tokenAfterOperator.Value.Contains('.'))
                    sqlObject = GetSqlObject(tokenAfterOperator.Value);
                else if (prev.Value == "=" && index >= 2)
                {
                    var tokenBeforeOperator = Tokens[index - 2];
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
                var value = previousToken.Value.ToLower();
                var name = currentToken != null
                    ? currentToken.Value
                    : null;

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
                        sqlObject = new SqlObject(null, null,
                            SqlObjectTypes.Table | SqlObjectTypes.View | SqlObjectTypes.Function, name);
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
                            sqlObject = GetSqlObject(currentToken.Value);

                        if (sqlObject == null)
                        {
                            var index = _allTables.LastIndexOf(t => t.Index < previousToken.Index - 1);
                            if (index >= 0)
                            {
                                var table = _allTables[index];
                                sqlObject = new SqlObject(table.Name, null, SqlObjectTypes.Column, name);
                            }
                        }

                        break;

                    default:
                        break;
                }

                if (sqlObject == null && currentToken != null)
                    sqlObject = GetSqlObject(currentToken.Value);
            }
            else if (previousToken.Type == TokenType.OperatorOrPunctuator)
            {
                if (previousToken.Value == "=" && previousToken.Index > 0)
                {
                    if (currentToken != null)
                    {
                        switch (currentToken.Type)
                        {
                            case TokenType.KeyWord:
                                sqlObject = GetSqlObject(currentToken.Value);
                                break;

                            default:
                                sqlObject = GetValue(previousToken);
                                break;
                        }
                    }
                    else
                        sqlObject = GetValue(previousToken);
                }
                else if (currentToken != null && currentToken.Type == TokenType.KeyWord &&
                         currentToken.Value.Contains('.'))
                    sqlObject = GetSqlObject(currentToken.Value);
            }
        }

        return sqlObject;
    }

    private SqlObject GetValue(Token previousToken)
    {
        var tokenBeforeOperator = Tokens[previousToken.Index - 1];
        return new SqlObject(tokenBeforeOperator.Value, null, SqlObjectTypes.Value, null);
    }

    public string FindTableName()
    {
        string tableName = null;
        int i;

        for (i = 0; i < Tokens.Count; i++)
        {
            var token = Tokens[i];
            if (token.Type == TokenType.KeyWord)
                if (string.Compare(token.Value, "from", true) == 0)
                    break;
        }

        i++;

        if (i < Tokens.Count)
        {
            var token = Tokens[i];
            if (token.Type == TokenType.KeyWord)
                tableName = token.Value;
        }

        return tableName;
    }

    public int FindToken(int position)
    {
        var index = -1;
        var last = Tokens.Count - 1;
        for (var i = 0; i <= last; i++)
        {
            var token = Tokens[i];

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
            var lastToken = Tokens[last];
            if (lastToken.EndPosition < position)
                index = last + 1;
        }

        return index;
    }

    public void FindToken(int position, out Token previousToken, out Token currentToken)
    {
        previousToken = null;
        currentToken = null;

        for (var i = 0; i < Tokens.Count; i++)
        {
            var token = Tokens[i];
            if (token.EndPosition + 1 < position)
                previousToken = token;
            else if (token.StartPosition <= position && position <= token.EndPosition + 1)
            {
                currentToken = token;
                break;
            }
            else
                break;
        }
    }

    #endregion

    #region Private Methods

    public static List<Token> Tokenize(string text)
    {
        var tokens = new List<Token>();
        var iterator = new TokenIterator(text);

        while (true)
        {
            var token = iterator.Next();

            if (token == null)
                break;

            tokens.Add(token);
        }

        return tokens;
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
                        var valueStr = (string)value;
                        var ok = double.TryParse(valueStr, NumberStyles.Any, null, out var valueDbl);
                        value2 = ok ? Convert.ToBoolean(valueDbl) : Convert.ToBoolean(value);
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
                        var decimalString = new DecimalString(value.ToString());

                        if (dataParameter.Precision != 0 && decimalString.Precision > dataParameter.Precision)
                            throw new Exception("Invalid precision");
                        else if (dataParameter.Scale != 0 && decimalString.Scale > dataParameter.Scale)
                            throw new Exception("Invalid scale");
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
                            throw new Exception("Length exceeds size of parameter");
                        else
                            value2 = valueStr;

                        break;

                    case DbType.DateTime:
                        try
                        {
                            value2 = Convert.ToDateTime(value);
                        }
                        catch
                        {
                            var formats = new[]
                            {
                                "yyyyMMdd",
                                "yyyyMMdd HH:mm:ss"
                            };

                            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                            value2 = DateTime.ParseExact(value.ToString(), formats, formatProvider,
                                DateTimeStyles.None);
                        }

                        break;

                    default:
                        value2 = value;
                        break;
                }
            }
        }
        else
            value2 = null;

        return value2;
    }

    private static object ToParameterValue(Token token)
    {
        var tokenValue = token.Value;
        object value;
        if (token.Type == TokenType.KeyWord &&
            string.Compare(tokenValue, "null", StringComparison.InvariantCultureIgnoreCase) == 0)
            value = DBNull.Value;
        else if (token.Type == TokenType.KeyWord &&
                 string.Compare(tokenValue, "default", StringComparison.InvariantCultureIgnoreCase) == 0)
            value = null;
        else
            value = tokenValue;

        return value;
    }

    private static Parameter ToParameter(List<Token> tokens)
    {
        string name;
        object value;
        var count = tokens.Count;
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
            throw new NotImplementedException();

        return new Parameter(name, value);
    }

    private static IDictionary<string, string> FindTables(List<Token> tokens, out Table[] allTables)
    {
        var tables = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        var tableList = new List<Table>();

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            var value = token.Value;

            if (token.Type == TokenType.KeyWord && value != null && i < tokens.Count - 1)
            {
                switch (value.ToLower())
                {
                    case "from":
                    case "join":
                        token = tokens[i + 1];

                        if (token.Type == TokenType.KeyWord)
                        {
                            var tableName = token.Value;

                            if (i < tokens.Count - 2)
                            {
                                token = tokens[i + 2];

                                if (token.Type == TokenType.KeyWord)
                                {
                                    var alias = token.Value;
                                    tableList.Add(new Table(i, tableName, alias));
                                    if (!tables.ContainsKey(alias))
                                        tables.Add(alias, tableName);
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
        var items = value.Split('.');
        if (items.Length > 1)
        {
            var alias = items[0];
            var contains = Tables.TryGetValue(alias, out var tableName);

            if (contains)
            {
                var name = items[1];
                sqlObject = new SqlObject(tableName, alias, SqlObjectTypes.Column, name);
            }
            else
                sqlObject = new SqlObject(null, null, SqlObjectTypes.Function, value);
        }

        return sqlObject;
    }

    #endregion

    #region Private Classes

    private sealed class Table
    {
        private string _alias;

        public readonly int Index;
        public readonly string Name;

        public Table(int index, string name, string alias)
        {
            Index = index;
            Name = name;
            _alias = alias;
        }
    }

    private sealed class Parameter
    {
        public readonly string Name;
        public readonly object Value;

        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }

    #endregion
}