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
        private static UniqueIndex<String, TypeCollectionItem> nameIndex;
        private static UniqueIndex<Type, TypeCollectionItem> typeIndex;
        private static Assembly systemAssembly;

        static TypeNameCollection()
        {
            nameIndex = new UniqueIndex<String, TypeCollectionItem>(
                "Name",
                item => GetKeyResponse.Create( true, item.Name ),
                SortOrder.None );

            typeIndex = new UniqueIndex<Type, TypeCollectionItem>(
                "Type",
                item => GetKeyResponse.Create( true, item.Type ),
                new Dictionary<Type, TypeCollectionItem>( TypeEqualityComparer.Instance ) );

            collection = new IndexableCollection<TypeCollectionItem>( nameIndex );
            collection.Indexes.Add( typeIndex );

            Add( TypeName.Bool, typeof( Boolean ) );
            Add( TypeName.Char, typeof( Char ) );
            Add( TypeName.String, typeof( String ) );
            Add( TypeName.Object, typeof( Object ) );

            Add( TypeName.SByte, typeof( SByte ) );
            Add( TypeName.Int16, typeof( Int16 ) );
            Add( TypeName.Int32, typeof( Int32 ) );
            Add( TypeName.Int64, typeof( Int64 ) );
            Add( TypeName.Byte, typeof( Byte ) );
            Add( TypeName.UInt16, typeof( UInt16 ) );
            Add( TypeName.UInt32, typeof( UInt32 ) );
            Add( TypeName.UInt64, typeof( UInt64 ) );

            Add( TypeName.Single, typeof( Single ) );
            Add( TypeName.Double, typeof( Double ) );
            Add( TypeName.Decimal, typeof( Decimal ) );

            Add( TypeName.DateTime, typeof( DateTime ) );
            Add( TypeName.XmlNode, typeof( XmlNode ) );

            systemAssembly = Assembly.GetAssembly( typeof( Int32 ) );
        }

        private static void Add( String name, Type type )
        {
            TypeCollectionItem item = new TypeCollectionItem( name, type );
            collection.Add( item );
        }

        public static Type GetType( String typeName )
        {
            Type type;
            Boolean isArray = false;
            Int32 length = typeName.Length - 2;
            isArray = typeName != null && typeName.IndexOf( "[]" ) == length;

            String typeName2;

            if (isArray)
            {
                typeName2 = typeName.Substring( 0, length );
            }
            else
            {
                typeName2 = typeName;
            }

            TypeCollectionItem item;
            Boolean contains = nameIndex.TryGetValue( typeName2, out item );

            if (contains)
            {
                type = item.Type;
            }
            else
            {
                type = Type.GetType( typeName2 );
            }

            if (type != null)
            {
                if (isArray)
                {
                    typeName2 = type.FullName + "[], " + type.Assembly.FullName;
                    type = Type.GetType( typeName2, true );
                }
            }

            return type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String GetTypeName( Type type )
        {
            String typeName;

            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                typeName = GetTypeName( elementType ) + "[]";
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
                Boolean contains = typeIndex.TryGetValue( type, out item );

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
            private String name;
            private Type type;

            public TypeCollectionItem( String name, Type type )
            {
                this.name = name;
                this.type = type;
            }

            public String Name
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
            public const String Bool = "bool";
            public const String Char = "char";
            public const String String = "string";
            public const String Object = "object";

            public const String SByte = "sbyte";
            public const String Int16 = "short";
            public const String Int32 = "int";
            public const String Int64 = "long";
            public const String Byte = "byte";
            public const String UInt16 = "ushort";
            public const String UInt32 = "uint";
            public const String UInt64 = "ulong";

            public const String Single = "float";
            public const String Double = "double";
            public const String Decimal = "decimal";

            public const String DateTime = "datetime";
            public const String XmlNode = "xmlnode";
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

            public Boolean Equals( Type x, Type y )
            {
                return x == y;
            }

            public Int32 GetHashCode( Type obj )
            {
                return obj.GetHashCode();
            }

            #endregion
        }
    }
}