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
            this.message = string.Format( "Conversion ({0}) of string value '{1}' to type {2} failed. ", converter, fieldValue, column.DataType );
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                return this.message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumn Column
        {
            get
            {
                return this.column;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ITextDataConverter Converter
        {
            get
            {
                return this.converter;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Fieldvalue
        {
            get
            {
                return this.fieldValue;
            }
        }
    }
}