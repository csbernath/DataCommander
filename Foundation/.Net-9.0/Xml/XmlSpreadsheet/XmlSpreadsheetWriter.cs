using System;
using System.Collections.Generic;
using System.Xml;

namespace Foundation.Xml.XmlSpreadsheet;

public sealed class XmlSpreadsheetWriter
{
    private XmlSpreadsheetTable _table;
    private int _tableIndex = -1;

    public XmlSpreadsheetWriter(XmlWriter xmlWriter)
    {
        ArgumentNullException.ThrowIfNull(xmlWriter);

        XmlWriter = xmlWriter;

        XmlWriter.WriteStartDocument();
        XmlWriter.WriteProcessingInstruction("mso-application", "progid=\"Excel.Sheet\"");
        XmlWriter.WriteStartElement("Workbook");
        XmlWriter.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:spreadsheet");
        XmlWriter.WriteAttributeString("xmlns:ss", "urn:schemas-microsoft-com:office:spreadsheet");
    }

    public XmlWriter XmlWriter { get; }

    public void WriteStyles(IEnumerable<XmlSpreadsheetTable> tables)
    {
        using (XmlWriter.WriteElement("Styles"))
        {
            using (XmlWriter.WriteElement("Style"))
            {
                XmlWriter.WriteAttributeString("ss:ID", "ColumnHeader");

                using (XmlWriter.WriteElement("Alignment"))
                {
                    XmlWriter.WriteAttributeString("ss:Vertical", "Top");
                    XmlWriter.WriteAttributeString("ss:WrapText", "1");
                }

                using (XmlWriter.WriteElement("Borders"))
                {
                    using (XmlWriter.WriteElement("Border"))
                    {
                        XmlWriter.WriteAttributeString("ss:Position", "Bottom");
                        XmlWriter.WriteAttributeString("ss:LineStyle", "Continuous");
                        XmlWriter.WriteAttributeString("ss:Weight", "1");
                    }
                }

                using (XmlWriter.WriteElement("Font"))
                {
                    XmlWriter.WriteAttributeString("ss:Bold", "1");
                }
            }

            int tableIndex = 0;
            foreach (XmlSpreadsheetTable tableSchema in tables)
            {
                int columnIndex = 0;
                foreach (XmlSpreadsheetColumn column in tableSchema.Columns)
                {
                    using (XmlWriter.WriteElement("Style"))
                    {
                        string id = $"{tableIndex},{columnIndex}";
                        XmlWriter.WriteAttributeString("ss:ID", id);

                        if (column.NumberFormat != null)
                        {
                            using (XmlWriter.WriteElement("NumberFormat"))
                            {
                                XmlWriter.WriteAttributeString("ss:Format", column.NumberFormat);
                            }
                        }
                    }

                    columnIndex++;
                }

                tableIndex++;
            }
        }
    }
    
    public void WriteStartTable(XmlSpreadsheetTable table)
    {
        ArgumentNullException.ThrowIfNull(table);
        _tableIndex++;
        _table = table;

        XmlWriter.WriteStartElement("Worksheet");
        XmlWriter.WriteAttributeString("ss:Name", _table.TableName);

        XmlWriter.WriteStartElement("Table");

        int columnIndex = 1;
        foreach (XmlSpreadsheetColumn column in _table.Columns)
        {
            using (XmlWriter.WriteElement("Column"))
            {
                XmlWriter.WriteAttributeString("ss:Index", columnIndex.ToString());

                if (column.Width != null)
                {
                    XmlWriter.WriteAttributeString("ss:Width", column.Width);
                }
            }

            columnIndex++;
        }

        using (XmlWriter.WriteElement("Row"))
        {
            foreach (XmlSpreadsheetColumn column in _table.Columns)
            {
                XmlSpreadsheetCell cell =
                    new XmlSpreadsheetCell(XmlSpreadsheetDataType.String, column.ColumnName)
                    {
                        StyleId = "ColumnHeader"
                    };
                cell.Write(XmlWriter);
            }
        }
    }
    
    public void WriteEndTable()
    {
        // Table
        XmlWriter.WriteEndElement();

        using (XmlWriter.WriteElement("WorksheetOptions"))
        {
            XmlWriter.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:excel");

            XmlWriter.WriteElementString("Selected", null);
            XmlWriter.WriteElementString("FreezePanes", null);
            XmlWriter.WriteElementString("FrozenNoSplit", null);
            XmlWriter.WriteElementString("SplitHorizontal", "1");
            XmlWriter.WriteElementString("TopRowBottomPane", "1");
            XmlWriter.WriteElementString("ActivePane", "2");
        }

        // Worksheet
        XmlWriter.WriteEndElement();
    }

    public void WriteStartRow() => XmlWriter.WriteStartElement("Row");

    public void WriteEndRow() => XmlWriter.WriteEndElement();

    public void WriteRow(object[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        WriteStartRow();

        for (int columnIndex = 0; columnIndex < values.Length; columnIndex++)
        {
            object value = values[columnIndex];
            XmlSpreadsheetDataType type;
            string xmlValue;

            if (value == DBNull.Value)
            {
                xmlValue = null;
                type = XmlSpreadsheetDataType.String;
            }
            else
            {
                XmlSpreadsheetColumn column = _table.Columns[columnIndex];
                type = column.DataType;
                xmlValue = column.Convert(value);
            }

            XmlSpreadsheetCell cell = new XmlSpreadsheetCell(type, xmlValue)
            {
                StyleId = $"{_tableIndex},{columnIndex}"
            };
            cell.Write(XmlWriter);
        }

        WriteEndRow();
    }
}