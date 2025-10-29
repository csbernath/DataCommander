using System;
using System.Collections.Generic;
using Npgsql.Internal.Postgres;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

[CLSCompliant(false)]
public class PostgresSqlNamespaceRepository(Dictionary<uint, PostgresSqlNamespace> namespaces)
{
    [CLSCompliant(false)]
    public bool TryGetByOid(uint oid, out PostgresSqlNamespace? @namespace) => namespaces.TryGetValue(oid, out @namespace);
}