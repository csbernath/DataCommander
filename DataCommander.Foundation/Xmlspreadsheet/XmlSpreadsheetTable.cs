namespace DataCommander.Foundation.XmlSpreadsheet
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlSpreadsheetTable
    {
        /// <summary>
        /// 
        /// </summary>
        public String TableName;
        
        /// <summary>
        /// 
        /// </summary>
        public List<XmlSpreadsheetColumn> Columns = new List<XmlSpreadsheetColumn>();
    }
}