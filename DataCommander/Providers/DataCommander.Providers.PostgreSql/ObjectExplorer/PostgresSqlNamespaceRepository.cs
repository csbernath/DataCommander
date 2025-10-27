using System;
using System.Collections.Generic;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

[CLSCompliant(false)]
public class PostgresSqlNamespaceRepository
{
    private readonly Dictionary<uint, PostgresSqlNamespace> _namespaces;

    public PostgresSqlNamespaceRepository(Dictionary<uint, PostgresSqlNamespace> namespaces)
    {
        _namespaces = namespaces;
    }

    [CLSCompliant(false)]
    public bool TryGetByOid(uint oit, out PostgresSqlNamespace? @namespace) => _namespaces.TryGetValue(oit, out @namespace);
}