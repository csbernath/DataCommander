using System.Diagnostics;
using System.Linq;
using System.Text;
using Foundation.Collections;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public sealed class DataTransferObjectField(string name, CSharpType cSharpType, bool allowNull, bool isArray)
{
    public readonly string Name = name;    
    public readonly CSharpType CSharpType = cSharpType;
    public readonly bool AllowNull = allowNull;    
    public readonly bool IsArray = isArray;

    public string GetTypeName()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(CSharpType.Name);

        if (AllowNull)
        {
            if (CSharpType.Name is
                CSharpTypeName.Boolean or
                CSharpTypeName.DateTime or
                CSharpTypeName.DateTimeOffset or
                CSharpTypeName.Decimal or
                CSharpTypeName.Double or
                CSharpTypeName.Guid or
                CSharpTypeName.Int16 or
                CSharpTypeName.Int32 or
                CSharpTypeName.Int64 or
                CSharpTypeName.Single or
                CSharpTypeName.String)
                stringBuilder.Append('?');
            else
                Debug.WriteLine("");
        }
        
        if (IsArray)
            stringBuilder.Append("[]");

        return stringBuilder.ToString();
    }
}