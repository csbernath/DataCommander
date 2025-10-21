using System.Collections.Generic;
using System.Linq;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

public class PostgresSqlTypeRepository
{
    public static readonly PostgresSqlType Boolean = new(16, "boolean");
    public static readonly PostgresSqlType Integer = new(23, "integer");
    public static readonly PostgresSqlType CharacterVarying = new(1043, "character varying");
    public static readonly PostgresSqlType TimestampWithTimeZone = new(1184, "timestamp with time zone");
    public static readonly PostgresSqlType RefCursor = new(1790, "refcursor");
    public static readonly PostgresSqlType Uuid = new(2950, "uuid");

    private static readonly PostgresSqlType[] Array =
    [
        Boolean,
        Integer,
        CharacterVarying,
        RefCursor,
        TimestampWithTimeZone,
        Uuid
    ];

    private static readonly Dictionary<uint, PostgresSqlType> ByOid = Array.ToDictionary(t => t.Oid);

    public static bool TryGetByOid(uint oid, out PostgresSqlType? postgresSqlType) => ByOid.TryGetValue(oid, out postgresSqlType);
}