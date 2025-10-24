using System.Collections.Generic;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

public class PostgresSqlNamespaceRepository
{
    private readonly Dictionary<uint, PostgresSqlNamespace> _namespaces;

    public PostgresSqlNamespaceRepository(Dictionary<uint, PostgresSqlNamespace> namespaces)
    {
        _namespaces = namespaces;
    }

    public bool TryGetByOid(uint oit, out PostgresSqlNamespace? @namespace) => _namespaces.TryGetValue(oit, out @namespace);
}