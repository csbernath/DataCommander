using System;
using System.Collections.Generic;

namespace Foundation.Xml.XmlSpreadsheet;

public sealed class XmlSpreadsheetColumn
{
    public string? ColumnName;
    public XmlSpreadsheetDataType DataType;
    public string? NumberFormat;
    public string? Width;
    public Converter<object, string>? Convert;
    public Dictionary<string, object>? ExtendedProperties = [];
}