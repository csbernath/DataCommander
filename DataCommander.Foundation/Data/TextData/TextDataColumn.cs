namespace DataCommander.Foundation.Data
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("ColumnName: {columnName}, DataType: {dataType}")]
    public class TextDataColumn
    {
        #region Private Fields

        private readonly String columnName;
        private String caption;
        private Int32 maxLength;
        private readonly Type dataType;
        private Byte? numericPrecision;
        private Byte? numericScale;
        private String format;
        private IFormatProvider formatProvider;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="maxLength"></param>
        /// <param name="dataType"></param>
        /// <param name="numericPrecision"></param>
        /// <param name="numericScale"></param>
        /// <param name="formatProvider"></param>
        /// <param name="format"></param>
        /// <param name="caption"></param>
        public TextDataColumn(
            String columnName,
            String caption,
            Int32 maxLength,
            Type dataType,
            Byte? numericPrecision,
            Byte? numericScale,
            String format,
            IFormatProvider formatProvider)
        {
            this.columnName = columnName;
            this.caption = caption;
            this.maxLength = maxLength;
            this.dataType = dataType;
            this.numericPrecision = numericPrecision;
            this.numericScale = numericScale;
            this.format = format;
            this.formatProvider = formatProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type DataType
        {
            get
            {
                return this.dataType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Format
        {
            get
            {
                return this.format;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IFormatProvider FormatProvider
        {
            get
            {
                return this.formatProvider;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 MaxLength
        {
            get
            {
                return this.maxLength;
            }
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
        public String Caption
        {
            get
            {
                return this.caption;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Byte? NumericPrecision
        {
            get
            {
                return this.numericPrecision;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Byte? NumericScale
        {
            get
            {
                return this.numericScale;
            }
        }
    }
}