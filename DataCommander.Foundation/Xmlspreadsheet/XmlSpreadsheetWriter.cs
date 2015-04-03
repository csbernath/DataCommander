namespace DataCommander.Foundation.XmlSpreadsheet
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Xml;
    using DataCommander.Foundation.Xml;

    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlSpreadsheetWriter
    {
        private XmlSpreadsheetTable table;
        private readonly XmlWriter xmlWriter;
        private int tableIndex = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        public XmlSpreadsheetWriter( XmlWriter xmlWriter )
        {
            Contract.Requires( xmlWriter != null );

            this.xmlWriter = xmlWriter;

            this.xmlWriter.WriteStartDocument();
            this.xmlWriter.WriteProcessingInstruction( "mso-application", "progid=\"Excel.Sheet\"" );
            this.xmlWriter.WriteStartElement( "Workbook" );
            this.xmlWriter.WriteAttributeString( "xmlns", "urn:schemas-microsoft-com:office:spreadsheet" );
            this.xmlWriter.WriteAttributeString( "xmlns:ss", "urn:schemas-microsoft-com:office:spreadsheet" );
        }

        /// <summary>
        /// 
        /// </summary>
        public XmlWriter XmlWriter
        {
            get
            {
                return this.xmlWriter;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public void WriteStyles( IEnumerable<XmlSpreadsheetTable> tables )
        {
            using (this.xmlWriter.WriteElement( "Styles" ))
            {
                using (this.xmlWriter.WriteElement( "Style" ))
                {
                    this.xmlWriter.WriteAttributeString( "ss:ID", "ColumnHeader" );

                    using (this.xmlWriter.WriteElement( "Alignment" ))
                    {
                        this.xmlWriter.WriteAttributeString( "ss:Vertical", "Top" );
                        this.xmlWriter.WriteAttributeString( "ss:WrapText", "1" );
                    }

                    using (this.xmlWriter.WriteElement( "Borders" ))
                    {
                        using (this.xmlWriter.WriteElement( "Border" ))
                        {
                            this.xmlWriter.WriteAttributeString( "ss:Position", "Bottom" );
                            this.xmlWriter.WriteAttributeString( "ss:LineStyle", "Continuous" );
                            this.xmlWriter.WriteAttributeString( "ss:Weight", "1" );
                        }
                    }

                    using (this.xmlWriter.WriteElement( "Font" ))
                    {
                        this.xmlWriter.WriteAttributeString( "ss:Bold", "1" );
                    }
                }

                int tableIndex = 0;
                foreach (var tableSchema in tables)
                {
                    int columnIndex = 0;
                    foreach (var column in tableSchema.Columns)
                    {
                        using (this.xmlWriter.WriteElement( "Style" ))
                        {
                            string id = string.Format( "{0},{1}", tableIndex, columnIndex );
                            this.xmlWriter.WriteAttributeString( "ss:ID", id );

                            if (column.NumberFormat != null)
                            {
                                using (this.xmlWriter.WriteElement( "NumberFormat" ))
                                {
                                    this.xmlWriter.WriteAttributeString( "ss:Format", column.NumberFormat );
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
        public void WriteStartTable( XmlSpreadsheetTable table )
        {
            Contract.Requires( table != null );
            this.tableIndex++;
            this.table = table;
            int columnIndex;

            this.xmlWriter.WriteStartElement( "Worksheet" );
            this.xmlWriter.WriteAttributeString( "ss:Name", this.table.TableName );

            this.xmlWriter.WriteStartElement( "Table" );

            columnIndex = 1;
            foreach (var column in this.table.Columns)
            {
                using (this.xmlWriter.WriteElement( "Column" ))
                {
                    this.xmlWriter.WriteAttributeString( "ss:Index", columnIndex.ToString() );

                    if (column.Width != null)
                    {
                        this.xmlWriter.WriteAttributeString( "ss:Width", column.Width );
                    }
                }
                columnIndex++;
            }

            using (this.xmlWriter.WriteElement( "Row" ))
            {
                foreach (var column in this.table.Columns)
                {
                    var cell =
                        new XmlSpreadsheetCell( XmlSpreadsheetDataType.String, column.ColumnName )
                        {
                            StyleId = "ColumnHeader"
                        };
                    cell.Write( this.xmlWriter );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteEndTable()
        {
            // Table
            this.xmlWriter.WriteEndElement();

            using (this.xmlWriter.WriteElement( "WorksheetOptions" ))
            {
                this.xmlWriter.WriteAttributeString( "xmlns", "urn:schemas-microsoft-com:office:excel" );

                this.xmlWriter.WriteElementString( "Selected", null );
                this.xmlWriter.WriteElementString( "FreezePanes", null );
                this.xmlWriter.WriteElementString( "FrozenNoSplit", null );
                this.xmlWriter.WriteElementString( "SplitHorizontal", "1" );
                this.xmlWriter.WriteElementString( "TopRowBottomPane", "1" );
                this.xmlWriter.WriteElementString( "ActivePane", "2" );
            }

            // Worksheet
            this.xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteStartRow()
        {
            this.xmlWriter.WriteStartElement( "Row" );
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteEndRow()
        {
            this.xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void WriteRow( object[] values )
        {
#if FONDATION_2_0 || FOUNDATION_3_5
            // TODO
#else
            Contract.Requires( values != null );
#endif

            this.WriteStartRow();

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
                    var column = this.table.Columns[columnIndex];
                    type = column.DataType;
                    xmlValue = column.Convert( value );
                }

                var cell = new XmlSpreadsheetCell( type, xmlValue );
                cell.StyleId = string.Format( "{0},{1}", this.tableIndex, columnIndex );
                cell.Write( this.xmlWriter );
            }

            this.WriteEndRow();
        }
    }
}