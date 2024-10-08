﻿using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql;

internal sealed class NonSqlObjectName : IObjectName
{
    private readonly string _objectName;

    public NonSqlObjectName(string objectName)
    {
        _objectName = objectName;
    }

    string IObjectName.UnquotedName => _objectName;

    string IObjectName.QuotedName => _objectName;
}