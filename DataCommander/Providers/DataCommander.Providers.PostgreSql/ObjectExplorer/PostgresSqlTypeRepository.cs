using System.Collections.Generic;
using System.Linq;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

public class PostgresSqlTypeRepository
{
    public static readonly PostgresSqlType Boolean = new PostgresSqlType(16, "boolean");
    public static readonly PostgresSqlType Integer = new PostgresSqlType(23, "integer");
    public static readonly PostgresSqlType CharacterVarying = new PostgresSqlType(1043, "character varying");
    public static readonly PostgresSqlType TimestampWithTimeZone = new PostgresSqlType(1184, "timestamp with time zone");
    public static readonly PostgresSqlType Uuid = new PostgresSqlType(2950, "uuid");

    private static PostgresSqlType[] _array =
    [
        Boolean,
        Integer,
        CharacterVarying,
        TimestampWithTimeZone,
        Uuid
    ];

    private static Dictionary<uint, PostgresSqlType> _byOid = _array.ToDictionary(t => t.Oid);

    public static bool TryGetByOid(uint oid, out PostgresSqlType? postgresSqlType) => _byOid.TryGetValue(oid, out postgresSqlType);
}