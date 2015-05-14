namespace DataCommander.Foundation.XmlSpreadsheet
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlSpreadsheetTable
    {
        /// <summary>
        /// 
        /// </summary>
        public string TableName;

        /// <summary>
        /// 
        /// </summary>
        public List<XmlSpreadsheetColumn> Columns = new List<XmlSpreadsheetColumn>();
    }
}