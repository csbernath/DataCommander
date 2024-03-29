﻿using System.Collections.ObjectModel;

namespace DataCommander.Api.QueryConfiguration;

public class Query
{
    public readonly string Name;
    public readonly string Using;
    public readonly string Namespace;
    public readonly ReadOnlyCollection<string> Results;
    public readonly int? CommandTimeout;

    public Query(string name, string @using, string @namespace, ReadOnlyCollection<string> results, int? commandTimeout)
    {
        Name = name;
        Using = @using;
        Namespace = @namespace;
        Results = results;
        CommandTimeout = commandTimeout;
    }
}