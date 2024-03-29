﻿using System.Collections.Generic;
using System.Data;

namespace Foundation.Data;

public class CreateCommandRequest
{
    public readonly string CommandText;
    public readonly IReadOnlyCollection<object> Parameters;
    public readonly CommandType CommandType;
    public readonly int? CommandTimeout;
    public readonly IDbTransaction Transaction;

    public CreateCommandRequest(
        string commandText,
        IReadOnlyCollection<object> parameters,
        CommandType commandType,
        int? commandTimeout,
        IDbTransaction transaction)
    {
        CommandText = commandText;
        Parameters = parameters;
        CommandType = commandType;
        CommandTimeout = commandTimeout;
        Transaction = transaction;
    }

    public CreateCommandRequest(string commandText, IReadOnlyCollection<object> parameters)
        : this(commandText, parameters, CommandType.Text, null, null)
    {
    }

    public CreateCommandRequest(string commandText)
        : this(commandText, null)
    {
    }
}