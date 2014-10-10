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
        public String ColumnName;
        
        /// <summary>
        /// 
        /// </summary>
        public XmlSpreadsheetDataType DataType;
        
        /// <summary>
        /// 
        /// </summary>
        public String NumberFormat;
        
        /// <summary>
        /// 
        /// </summary>
        public String Width;

        /// <summary>
        /// 
        /// </summary>
        public Converter<Object, String> Convert;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<String, Object> ExtendedProperties = new Dictionary<String, Object>();
    }
}