using System;
using System.Xml;

namespace Foundation.Xml.XmlSpreadsheet;

public sealed class XmlSpreadsheetCell
{
    private readonly XmlSpreadsheetAttributeCollection _attributes = new();
    private readonly XmlSpreadsheetDataType _dataType;
    private readonly string _value;

    public XmlSpreadsheetCell(XmlSpreadsheetDataType dataType, string value)
    {
        _dataType = dataType;
        _value = value;
    }

    public string StyleId
    {
        set
        {
            var attribute = new XmlSpreadsheetAttribute("ss:StyleID", value);
            _attributes.Add(attribute);
        }
    }

    public int MergeAcross
    {
        set
        {
            var attribute = new XmlSpreadsheetAttribute("ss:MergeAcross", value.ToString());
            _attributes.Add(attribute);
        }
    }

    public void Write(XmlWriter xmlWriter)
    {
        ArgumentNullException.ThrowIfNull(xmlWriter);

        using (xmlWriter.WriteElement("Cell"))
        {
            foreach (var attribute in _attributes)
            {
                attribute.Write(xmlWriter);
            }

            using (xmlWriter.WriteElement("Data"))
            {
                xmlWriter.WriteAttributeString("ss:Type", _dataType.ToString());
                xmlWriter.WriteString(_value);
            }
        }
    }
}