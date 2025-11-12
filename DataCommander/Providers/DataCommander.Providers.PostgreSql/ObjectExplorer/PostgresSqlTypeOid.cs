using System;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

[CLSCompliant(false)]
public static class PostgresSqlTypeOid
{
    public const uint Boolean = 16;
    public const uint Integer = 23;
    public const uint CharacterVarying = 1043;
    public const uint TimestampWithTimeZone = 1184;
    public const uint RefCursor = 1790;
    public const uint Uuid = 2950;
}