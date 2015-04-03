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

        private readonly string columnName;
        private readonly string caption;
        private readonly int maxLength;
        private readonly Type dataType;
        private readonly byte? numericPrecision;
        private readonly byte? numericScale;
        private readonly string format;
        private readonly IFormatProvider formatProvider;

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
            string columnName,
            string caption,
            int maxLength,
            Type dataType,
            byte? numericPrecision,
            byte? numericScale,
            string format,
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
        public string Format
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
        public int MaxLength
        {
            get
            {
                return this.maxLength;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName
        {
            get
            {
                return this.columnName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Caption
        {
            get
            {
                return this.caption;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte? NumericPrecision
        {
            get
            {
                return this.numericPrecision;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte? NumericScale
        {
            get
            {
                return this.numericScale;
            }
        }
    }
}