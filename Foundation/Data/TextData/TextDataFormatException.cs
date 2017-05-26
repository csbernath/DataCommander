using System;

namespace Foundation.Data.TextData
{
    /// <summary>
    /// 
    /// </summary>
    public class TextDataFormatException : FormatException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="converter"></param>
        /// <param name="fieldValue"></param>
        /// <param name="innerException"></param>
        public TextDataFormatException( TextDataColumn column, ITextDataConverter converter, string fieldValue, Exception innerException )
            : base( null, innerException )
        {
            this.Column = column;
            this.Converter = converter;
            this.Fieldvalue = fieldValue;
            this.Message = $"Conversion ({converter}) of string value '{fieldValue}' to type {column.DataType} failed. ";
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message { get; }

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumn Column { get; }

        /// <summary>
        /// 
        /// </summary>
        public ITextDataConverter Converter { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Fieldvalue { get; }
    }
}