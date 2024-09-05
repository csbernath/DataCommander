using System.Collections.Generic;

namespace Foundation.Xml.XmlSpreadsheet;

public sealed class XmlSpreadsheetTable
{
    public string TableName;
    public List<XmlSpreadsheetColumn> Columns = [];
}