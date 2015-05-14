namespace DataCommander.Foundation.XmlSpreadsheet
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlSpreadsheetColumn
    {
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName;

        /// <summary>
        /// 
        /// </summary>
        public XmlSpreadsheetDataType DataType;

        /// <summary>
        /// 
        /// </summary>
        public string NumberFormat;

        /// <summary>
        /// 
        /// </summary>
        public string Width;

        /// <summary>
        /// 
        /// </summary>
        public Converter<object, string> Convert;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> ExtendedProperties = new Dictionary<string, object>();
    }
}