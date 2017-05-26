using System;
using System.Diagnostics;

namespace Foundation.Data.TextData
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("ColumnName: {ColumnName}, DataType: {DataType}")]
    public class TextDataColumn
    {
        #region Private Fields

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
            this.ColumnName = columnName;
            this.Caption = caption;
            this.MaxLength = maxLength;
            this.DataType = dataType;
            this.NumericPrecision = numericPrecision;
            this.NumericScale = numericScale;
            this.Format = format;
            this.FormatProvider = formatProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type DataType { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// 
        /// </summary>
        public IFormatProvider FormatProvider { get; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxLength { get; }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Caption { get; }

        /// <summary>
        /// 
        /// </summary>
        public byte? NumericPrecision { get; }

        /// <summary>
        /// 
        /// </summary>
        public byte? NumericScale { get; }
    }
}