using System;
using System.Xml;

namespace Foundation.Xml.XmlSpreadsheet;

public sealed class XmlSpreadsheetCell(XmlSpreadsheetDataType dataType, string value)
{
    private readonly XmlSpreadsheetAttributeCollection _attributes = [];

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
                xmlWriter.WriteAttributeString("ss:Type", dataType.ToString());
                xmlWriter.WriteString(value);
            }
        }
    }
}