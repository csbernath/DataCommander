namespace DataCommander.Foundation.Data
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class TextDataFormatException : FormatException
    {
        private readonly string message;
        private readonly TextDataColumn column;
        private readonly ITextDataConverter converter;
        private readonly string fieldValue;

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
            this.column = column;
            this.converter = converter;
            this.fieldValue = fieldValue;
            this.message = $"Conversion ({converter}) of string value '{fieldValue}' to type {column.DataType} failed. ";
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message => this.message;

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumn Column => this.column;

        /// <summary>
        /// 
        /// </summary>
        public ITextDataConverter Converter => this.converter;

        /// <summary>
        /// 
        /// </summary>
        public string Fieldvalue => this.fieldValue;
    }
}