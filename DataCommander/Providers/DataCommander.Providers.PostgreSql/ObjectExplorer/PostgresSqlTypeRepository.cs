using System;
using System.Collections.Generic;
using System.Linq;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

[CLSCompliant(false)]
public class PostgresSqlTypeRepository
{
    private readonly Dictionary<uint, PostgresSqlType> _postgresSqlTypes;

    public readonly PostgresSqlTypeName Boolean;
    public readonly PostgresSqlTypeName Integer;
    public readonly PostgresSqlTypeName CharacterVarying;
    public readonly PostgresSqlTypeName TimestampWithTimeZone;
    public readonly PostgresSqlTypeName RefCursor;
    public readonly PostgresSqlTypeName Uuid;

    private readonly Dictionary<uint, PostgresSqlTypeName> _postgresSqlTypeNames;

    public PostgresSqlTypeRepository(Dictionary<uint, PostgresSqlType> postgresSqlTypes)
    {
        _postgresSqlTypes = postgresSqlTypes;

        Boolean = new PostgresSqlTypeName(postgresSqlTypes[PostgresSqlTypeOid.Boolean], "boolean");
        Integer = new PostgresSqlTypeName(postgresSqlTypes[PostgresSqlTypeOid.Integer], "integer");
        CharacterVarying = new PostgresSqlTypeName(postgresSqlTypes[PostgresSqlTypeOid.CharacterVarying], "character varying");
        TimestampWithTimeZone = new PostgresSqlTypeName(postgresSqlTypes[PostgresSqlTypeOid.TimestampWithTimeZone], "timestamp with time zone");
        RefCursor = new PostgresSqlTypeName(postgresSqlTypes[PostgresSqlTypeOid.RefCursor], "refcursor");
        Uuid = new PostgresSqlTypeName(postgresSqlTypes[PostgresSqlTypeOid.Uuid], "uuid");

        _postgresSqlTypeNames = new[]
        {
            Boolean,
            Integer,
            CharacterVarying,
            TimestampWithTimeZone,
            RefCursor,
            Uuid
        }.ToDictionary(n => n.Type.Oid);
    }

    public bool TryGetByOid(uint oid, out PostgresSqlType? postgresSqlType) => _postgresSqlTypes.TryGetValue(oid, out postgresSqlType);

    public bool TryGetPostgresSqlTypeName(uint oid, out PostgresSqlTypeName? postgresSqlTypeName) =>
        _postgresSqlTypeNames.TryGetValue(oid, out postgresSqlTypeName);
}