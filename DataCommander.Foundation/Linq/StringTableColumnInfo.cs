namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Text;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class StringTableColumnInfo<T>
    {
        private readonly String columnName;
        private readonly StringTableColumnAlign align;
        private readonly Func<T, Int32, String> toString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="align"></param>
        /// <param name="toString"></param>
        public StringTableColumnInfo(
            String columnName,
            StringTableColumnAlign align,
            Func<T, Int32, String> toString )
        {
            Contract.Requires<ArgumentNullException>( toString != null );

            this.columnName = columnName;
            this.align = align;
            this.toString = toString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="align"></param>
        /// <param name="getValue"></param>
        public StringTableColumnInfo(
            String columnName,
            StringTableColumnAlign align,
            Func<T, Object> getValue )
        {
            Contract.Requires<ArgumentNullException>( getValue != null );

            this.columnName = columnName;
            this.align = align;
            this.toString = delegate( T item, Int32 index )
            {
                Object value = getValue( item );
                return ToString( value );
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public String ColumnName
        {
            get
            {
                return this.columnName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public StringTableColumnAlign Align
        {
            get
            {
                return this.align;
            }
        }

        internal String ToString( T item, Int32 index )
        {
            return this.toString( item, index );
        }

        private static String ToString( Object source )
        {
            String result;
            if (source != null)
            {
                result = source.ToString();
            }
            else
            {
                result = "null";
            }
            return result;
        }
    }
}