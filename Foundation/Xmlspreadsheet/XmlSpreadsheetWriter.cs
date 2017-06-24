using System;
using System.Collections.Generic;
using System.Xml;
using Foundation.Xml;

namespace Foundation.XmlSpreadsheet
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlSpreadsheetWriter
    {
        private XmlSpreadsheetTable _table;
        private int _tableIndex = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        public XmlSpreadsheetWriter(XmlWriter xmlWriter)
        {
#if CONTRACTS_FULL
            Contract.Requires(xmlWriter != null);
#endif

            XmlWriter = xmlWriter;

            XmlWriter.WriteStartDocument();
            XmlWriter.WriteProcessingInstruction("mso-application", "progid=\"Excel.Sheet\"");
            XmlWriter.WriteStartElement("Workbook");
            XmlWriter.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:spreadsheet");
            XmlWriter.WriteAttributeString("xmlns:ss", "urn:schemas-microsoft-com:office:spreadsheet");
        }

        /// <summary>
        /// 
        /// </summary>
        public XmlWriter XmlWriter { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
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

                var tableIndex = 0;
                foreach (var tableSchema in tables)
                {
                    var columnIndex = 0;
                    foreach (var column in tableSchema.Columns)
                    {
                        using (XmlWriter.WriteElement("Style"))
                        {
                            var id = $"{tableIndex},{columnIndex}";
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void WriteStartTable(XmlSpreadsheetTable table)
        {
#if CONTRACTS_FULL
            Contract.Requires(table != null);
#endif
            _tableIndex++;
            _table = table;
            int columnIndex;

            XmlWriter.WriteStartElement("Worksheet");
            XmlWriter.WriteAttributeString("ss:Name", _table.TableName);

            XmlWriter.WriteStartElement("Table");

            columnIndex = 1;
            foreach (var column in _table.Columns)
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
                foreach (var column in _table.Columns)
                {
                    var cell =
                        new XmlSpreadsheetCell(XmlSpreadsheetDataType.String, column.ColumnName)
                        {
                            StyleId = "ColumnHeader"
                        };
                    cell.Write(XmlWriter);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public void WriteStartRow()
        {
            XmlWriter.WriteStartElement("Row");
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteEndRow()
        {
            XmlWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void WriteRow(object[] values)
        {
#if CONTRACTS_FULL
            Contract.Requires(values != null);
#endif

            WriteStartRow();

            for (var columnIndex = 0; columnIndex < values.Length; columnIndex++)
            {
                var value = values[columnIndex];
                XmlSpreadsheetDataType type;
                string xmlValue;

                if (value == DBNull.Value)
                {
                    xmlValue = null;
                    type = XmlSpreadsheetDataType.String;
                }
                else
                {
                    var column = _table.Columns[columnIndex];
                    type = column.DataType;
                    xmlValue = column.Convert(value);
                }

                var cell = new XmlSpreadsheetCell(type, xmlValue);
                cell.StyleId = $"{_tableIndex},{columnIndex}";
                cell.Write(XmlWriter);
            }

            WriteEndRow();
        }
    }
}