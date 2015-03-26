namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;
    using DataCommander.Foundation.Collections;

    /// <summary>
    /// Summary description for TypeNames.
    /// </summary>
    internal static class TypeNameCollection
    {
        private static IndexableCollection<TypeCollectionItem> collection;
        private static UniqueIndex<string, TypeCollectionItem> nameIndex;
        private static UniqueIndex<Type, TypeCollectionItem> typeIndex;
        private static Assembly systemAssembly;

        static TypeNameCollection()
        {
            nameIndex = new UniqueIndex<string, TypeCollectionItem>(
                "Name",
                item => GetKeyResponse.Create(true, item.Name),
                SortOrder.None);

            typeIndex = new UniqueIndex<Type, TypeCollectionItem>(
                "Type",
                item => GetKeyResponse.Create(true, item.Type),
                new Dictionary<Type, TypeCollectionItem>(TypeEqualityComparer.Instance));

            collection = new IndexableCollection<TypeCollectionItem>(nameIndex);
            collection.Indexes.Add(typeIndex);

            Add(TypeName.Bool, typeof (bool));
            Add(TypeName.Char, typeof (Char));
            Add(TypeName.String, typeof (string));
            Add(TypeName.Object, typeof (object));

            Add(TypeName.SByte, typeof (SByte));
            Add(TypeName.Int16, typeof (Int16));
            Add(TypeName.Int32, typeof (int));
            Add(TypeName.Int64, typeof (long));
            Add(TypeName.Byte, typeof (Byte));
            Add(TypeName.UInt16, typeof (UInt16));
            Add(TypeName.UInt32, typeof (UInt32));
            Add(TypeName.UInt64, typeof (UInt64));

            Add(TypeName.Single, typeof (Single));
            Add(TypeName.Double, typeof (Double));
            Add(TypeName.Decimal, typeof (Decimal));

            Add(TypeName.DateTime, typeof (DateTime));
            Add(TypeName.XmlNode, typeof (XmlNode));

            systemAssembly = Assembly.GetAssembly(typeof (int));
        }

        private static void Add(string name, Type type)
        {
            TypeCollectionItem item = new TypeCollectionItem(name, type);
            collection.Add(item);
        }

        public static Type GetType(string typeName)
        {
            Type type;
            bool isArray = false;
            int length = typeName.Length - 2;
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
            bool contains = nameIndex.TryGetValue(typeName2, out item);

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
                Type elementType = type.GetElementType();
                typeName = GetTypeName(elementType) + "[]";
            }
            else if (type.IsEnum)
            {
                Assembly assembly = type.Assembly;

                if (assembly == systemAssembly)
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
                bool contains = typeIndex.TryGetValue(type, out item);

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
            private string name;
            private Type type;

            public TypeCollectionItem(string name, Type type)
            {
                this.name = name;
                this.type = type;
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public Type Type
            {
                get
                {
                    return this.type;
                }
            }
        }

        private static class TypeName
        {
            public const string Bool = "bool";
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

            public const string DateTime = "datetime";
            public const string XmlNode = "xmlnode";
        }

        private sealed class TypeEqualityComparer : IEqualityComparer<Type>
        {
            private static TypeEqualityComparer instance = new TypeEqualityComparer();

            private TypeEqualityComparer()
            {
            }

            public static TypeEqualityComparer Instance
            {
                get
                {
                    return instance;
                }
            }

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