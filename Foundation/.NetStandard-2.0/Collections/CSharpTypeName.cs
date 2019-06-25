using System;
using Foundation.Collections.ReadOnly;

namespace Foundation.Collections
{
    public static class CSharpTypeName
    {
        public const string Boolean = "bool";
        public const string Char = "char";
        public const string String = "string";
        public const string Object = "object";

        public const string SByte = "sbyte";
        public const string Int16 = "short";
        public const string Int32 = "int";
        public const string Int64 = "long";

        public const string Byte = "byte";
        public const string UInt16 = "ushort";
        public const string UInt32 = "uint";
        public const string UInt64 = "ulong";

        public const string Single = "float";
        public const string Double = "double";
        public const string Decimal = "decimal";
    }

    public sealed class CSharpType
    {
        public readonly string Name;
        public readonly Type Type;

        public CSharpType(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }

    public static class CSharpTypeArray
    {
        public static readonly ReadOnlyArray<CSharpType> CSharpTypes = new ReadOnlyArray<CSharpType>(new[]
        {
            new CSharpType(CSharpTypeName.Boolean, typeof(bool)),
            new CSharpType(CSharpTypeName.Byte + "[]", typeof(byte[])),
            new CSharpType(CSharpTypeName.Char, typeof(char)),
            new CSharpType(CSharpTypeName.Decimal, typeof(decimal)),
            new CSharpType(CSharpTypeName.Int32, typeof(int)),
            new CSharpType(CSharpTypeName.String, typeof(string)),
            new CSharpType(nameof(DateTime), typeof(DateTime)),
            new CSharpType(nameof(Guid), typeof(Guid))
        });
    }
}