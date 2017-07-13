using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Data
{
    public sealed class OrmBuilder
    {
        private readonly bool _properties;
        private readonly StringBuilder _objectStringBuilder = new StringBuilder();
        private readonly StringBuilder _readObjectStringBuilder = new StringBuilder();

        public OrmBuilder(bool properties)
        {
            _properties = properties;
        }

        public void Add(string objectTypeName, IReadOnlyCollection<DbColumn> columns)
        {
            AddObject(objectTypeName, columns, _objectStringBuilder, _properties);
            AddReadObject(objectTypeName, columns, _readObjectStringBuilder);
        }

        private static void AddObject(string objectTypeName, IReadOnlyCollection<DbColumn> columns, StringBuilder stringBuilder, bool properties)
        {
            if (stringBuilder.Length > 0)
                stringBuilder.Append("\r\n\r\n");

            stringBuilder.Append("internal sealed class ");
            stringBuilder.Append(objectTypeName);
            stringBuilder.Append("\r\n{\r\n");

            var first = true;
            foreach (var column in columns)
            {
                if (first)
                    first = false;
                else
                    stringBuilder.AppendLine();

                stringBuilder.Append("    public ");
                stringBuilder.Append(GetCSharpTypeName(column.DataType));

                if (column.AllowDbNull == true && IsValueType(column.DataType))
                    stringBuilder.Append('?');

                stringBuilder.Append(' ');
                stringBuilder.Append(column.ColumnName);
                stringBuilder.Append(properties ? " { get; set; }" : ";");
            }

            stringBuilder.Append(@"
}");
        }

        private static void AddReadObject(string objectTypeName, IReadOnlyCollection<DbColumn> columns, StringBuilder stringBuilder)
        {
            const string objectInstanceName = "@object";

            if (stringBuilder.Length > 0)
                stringBuilder.Append("\r\n\r\n");

            stringBuilder.AppendFormat(@"private static {0} Read{0}(IDataRecord dataRecord)
{{
    var {1} = new {0}();
", objectTypeName, objectInstanceName);

            var first = true;
            var index = 0;
            foreach (var column in columns)
            {
                if (first)
                    first = false;
                else
                    stringBuilder.AppendLine();

                stringBuilder.AppendFormat("    {0}.", objectInstanceName);
                stringBuilder.Append(column.ColumnName);
                stringBuilder.Append(" = dataRecord.");
                stringBuilder.Append(GetDataRecordMethodName(column));
                stringBuilder.Append('(');
                stringBuilder.Append(index);
                stringBuilder.Append(");");

                ++index;
            }

            stringBuilder.AppendFormat(@"
    return {0};
}}", objectInstanceName);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(_objectStringBuilder);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.Append(_readObjectStringBuilder);
            return stringBuilder.ToString();
        }

        private static string GetCSharpTypeName(Type dbColumnDataType)
        {
            var typeCode = Type.GetTypeCode(dbColumnDataType);
            string csharpTypeName;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    csharpTypeName = CSharpTypeName.Boolean;
                    break;
                case TypeCode.Char:
                    csharpTypeName = CSharpTypeName.Char;
                    break;
                case TypeCode.SByte:
                    csharpTypeName = CSharpTypeName.SByte;
                    break;
                case TypeCode.Byte:
                    csharpTypeName = CSharpTypeName.Byte;
                    break;
                case TypeCode.Int16:
                    csharpTypeName = CSharpTypeName.Int16;
                    break;
                case TypeCode.UInt16:
                    csharpTypeName = CSharpTypeName.UInt16;
                    break;
                case TypeCode.Int32:
                    csharpTypeName = CSharpTypeName.Int32;
                    break;
                case TypeCode.UInt32:
                    csharpTypeName = CSharpTypeName.UInt32;
                    break;
                case TypeCode.Int64:
                    csharpTypeName = CSharpTypeName.Int64;
                    break;
                case TypeCode.UInt64:
                    csharpTypeName = CSharpTypeName.UInt64;
                    break;
                case TypeCode.Single:
                    csharpTypeName = CSharpTypeName.Single;
                    break;
                case TypeCode.Double:
                    csharpTypeName = CSharpTypeName.Double;
                    break;
                case TypeCode.Decimal:
                    csharpTypeName = CSharpTypeName.Decimal;
                    break;
                case TypeCode.String:
                    csharpTypeName = CSharpTypeName.String;
                    break;
                default:
                    csharpTypeName = dbColumnDataType.Name;
                    break;
            }
            return csharpTypeName;
        }

        private static bool IsValueType(Type dbColumnDataType)
        {
            var typeCode = Type.GetTypeCode(dbColumnDataType);
            var isValueType = false;

            switch (typeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    isValueType = true;
                    break;
            }

            return isValueType;
        }

        private static string GetDataRecordMethodName(DbColumn column)
        {
            var typeCode = Type.GetTypeCode(column.DataType);
            string methodName = null;
            switch (typeCode)
            {
                case TypeCode.Empty:
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.Boolean:
                    methodName = column.AllowDbNull == true ? "GetNullableBoolean" : "GetBoolean";
                    break;
                case TypeCode.Char:
                    break;
                case TypeCode.SByte:
                    break;
                case TypeCode.Byte:
                    methodName = column.AllowDbNull == true ? "GetNullableByte" : "GetByte";
                    break;
                case TypeCode.Int16:
                    methodName = column.AllowDbNull == true ? "GetNullableInt16" : "GetInt16";
                    break;
                case TypeCode.UInt16:
                    break;
                case TypeCode.Int32:
                    methodName = column.AllowDbNull == true ? "GetNullableInt32" : "GetInt32";
                    break;
                case TypeCode.UInt32:
                    break;
                case TypeCode.Int64:
                    methodName = column.AllowDbNull == true ? "GetNullableInt64" : "GetInt64";
                    break;
                case TypeCode.UInt64:
                    break;
                case TypeCode.Single:
                    methodName = column.AllowDbNull == true ? "GetNullableFloat" : "GetFloat";
                    break;
                case TypeCode.Double:
                    methodName = column.AllowDbNull == true ? "GetNullableDouble" : "GetDouble";
                    break;
                case TypeCode.Decimal:
                    methodName = column.AllowDbNull == true ? "GetNullableDecimal" : "GetDecimal";
                    break;
                case TypeCode.DateTime:
                    methodName = column.AllowDbNull == true ? "GetNullableDateTime" : "GetDateTime";
                    break;
                case TypeCode.String:
                    methodName = column.AllowDbNull == true ? "GetStringOrDefault" : "GetString";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return methodName;
        }
    }
}