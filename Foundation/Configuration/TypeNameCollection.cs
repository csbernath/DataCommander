using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Foundation.Collections.IndexableCollection;
using Foundation.Data;

namespace Foundation.Configuration
{
    /// <summary>
    /// Summary description for TypeNames.
    /// </summary>
    internal static class TypeNameCollection
    {
        private static readonly IndexableCollection<TypeCollectionItem> Collection;
        private static readonly UniqueIndex<string, TypeCollectionItem> NameIndex;
        private static readonly UniqueIndex<Type, TypeCollectionItem> TypeIndex;
        private static readonly Assembly SystemAssembly;

        static TypeNameCollection()
        {
            NameIndex = new UniqueIndex<string, TypeCollectionItem>(
                "Name",
                item => GetKeyResponse.Create(true, item.Name),
                SortOrder.None);

            TypeIndex = new UniqueIndex<Type, TypeCollectionItem>(
                "Type",
                item => GetKeyResponse.Create(true, item.Type),
                new Dictionary<Type, TypeCollectionItem>(TypeEqualityComparer.Instance));

            Collection = new IndexableCollection<TypeCollectionItem>(NameIndex);
            Collection.Indexes.Add(TypeIndex);

            Add(CSharpTypeName.Boolean, typeof(bool));
            Add(CSharpTypeName.Char, typeof(char));
            Add(CSharpTypeName.String, typeof(string));
            Add(CSharpTypeName.Object, typeof(object));

            Add(CSharpTypeName.SByte, typeof(sbyte));
            Add(CSharpTypeName.Int16, typeof(short));
            Add(CSharpTypeName.Int32, typeof(int));
            Add(CSharpTypeName.Int64, typeof(long));
            Add(CSharpTypeName.Byte, typeof(byte));
            Add(CSharpTypeName.UInt16, typeof(ushort));
            Add(CSharpTypeName.UInt32, typeof(uint));
            Add(CSharpTypeName.UInt64, typeof(ulong));

            Add(CSharpTypeName.Single, typeof(float));
            Add(CSharpTypeName.Double, typeof(double));
            Add(CSharpTypeName.Decimal, typeof(decimal));

            Add(TypeName.DateTime, typeof(DateTime));
            Add(TypeName.XmlNode, typeof(XmlNode));

            SystemAssembly = Assembly.GetAssembly(typeof(int));
        }

        private static void Add(string name, Type type)
        {
            var item = new TypeCollectionItem(name, type);
            Collection.Add(item);
        }

        public static Type GetType(string typeName)
        {
            Type type;
            var isArray = false;
            var length = typeName.Length - 2;
            isArray = typeName != null && typeName.IndexOf("[]") == length;

            string typeName2;

            if (isArray)
            {
                typeName2 = typeName.Substring(0, length);
            }
            else
            {
                typeName2 = typeName;
            }

            TypeCollectionItem item;
            var contains = NameIndex.TryGetValue(typeName2, out item);

            if (contains)
            {
                type = item.Type;
            }
            else
            {
                type = Type.GetType(typeName2);
            }

            if (type != null)
            {
                if (isArray)
                {
                    typeName2 = type.FullName + "[], " + type.Assembly.FullName;
                    type = Type.GetType(typeName2, true);
                }
            }

            return type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeName(Type type)
        {
            string typeName;

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                typeName = GetTypeName(elementType) + "[]";
            }
            else if (type.IsEnum)
            {
                var assembly = type.Assembly;

                if (assembly == SystemAssembly)
                {
                    typeName = type.FullName;
                }
                else
                {
                    // AssemblyName assemblyName = assembly.GetName();
                    // typeName = type.FullName + "," + assemblyName.Name;
                    typeName = type.AssemblyQualifiedName;
                }
            }
            else
            {
                TypeCollectionItem item;
                var contains = TypeIndex.TryGetValue(type, out item);

                if (contains)
                {
                    typeName = item.Name;
                }
                else
                {
                    typeName = type.FullName;
                }
            }

            return typeName;
        }

        private sealed class TypeCollectionItem
        {
            public TypeCollectionItem(string name, Type type)
            {
                Name = name;
                Type = type;
            }

            public string Name { get; }

            public Type Type { get; }
        }

        private static class TypeName
        {
            public const string DateTime = "datetime";
            public const string XmlNode = "xmlnode";
        }

        private sealed class TypeEqualityComparer : IEqualityComparer<Type>
        {
            private TypeEqualityComparer()
            {
            }

            public static TypeEqualityComparer Instance { get; } = new TypeEqualityComparer();

            #region IEqualityComparer<Type> Members

            public bool Equals(Type x, Type y)
            {
                return x == y;
            }

            public int GetHashCode(Type obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }
    }
}