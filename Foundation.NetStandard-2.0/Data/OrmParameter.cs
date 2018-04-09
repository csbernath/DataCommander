using System.Data;

namespace Foundation.Data
{
    /// <summary>
    /// @hrRelationshipTypeId,int,int?
    /// </summary>
    public sealed class OrmParameter
    {
        public readonly string SqlParameterName;
        public readonly SqlDbType? SqlDbType;
        public readonly string CSharpTypeName;

        public OrmParameter(string sqlParameterName, SqlDbType? sqlDbType, string cSharpTypeName)
        {
            SqlParameterName = sqlParameterName;
            SqlDbType = sqlDbType;
            CSharpTypeName = cSharpTypeName;
        }
    }
}