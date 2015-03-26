namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.XmlSpreadsheet;

    internal sealed class XmlSpreadsheetResultWriter : IResultWriter
    {
        private IProvider provider;
        private Action<InfoMessage> addInfoMessage;
        private IResultWriter logResultWriter;
        private XmlSpreadsheetWriter xmlSpreadSheetWriter;
        private ResultDataSet resultDataSet;
        private ResultDataTable resultDataTable;
        private string fileName;

        public XmlSpreadsheetResultWriter(
            IProvider provider,
            Action<InfoMessage> addInfoMessage )
        {
            Contract.Requires( provider != null );
            Contract.Requires( addInfoMessage != null );
            this.provider = provider;
            this.addInfoMessage = addInfoMessage;
            this.logResultWriter = new LogResultWriter( addInfoMessage );
        }

        #region IResultWriter Members

        void IResultWriter.Begin()
        {
            this.logResultWriter.Begin();
        }

        void IResultWriter.BeforeExecuteReader( IProvider provider, IDbCommand command )
        {
            this.logResultWriter.BeforeExecuteReader( provider, command );
        }

        void IResultWriter.AfterExecuteReader()
        {
            this.logResultWriter.AfterExecuteReader();
        }

        void IResultWriter.AfterCloseReader( int affectedRows )
        {
            this.logResultWriter.AfterCloseReader( affectedRows );
        }

        void IResultWriter.WriteTableBegin( DataTable schemaTable, string[] dataTypeNames )
        {
            this.logResultWriter.WriteTableBegin( schemaTable, dataTypeNames );

            if (this.xmlSpreadSheetWriter == null)
            {
                this.fileName = Path.GetTempFileName() + ".xml";
                var xmlTextWriter = new XmlTextWriter( fileName, Encoding.UTF8 );
                xmlTextWriter.Indentation = 2;
                xmlTextWriter.IndentChar = ' ';
                xmlTextWriter.Formatting = Formatting.Indented;

                this.xmlSpreadSheetWriter = new XmlSpreadsheetWriter( xmlTextWriter );

                this.resultDataSet = new ResultDataSet();
            }

            this.resultDataTable = new ResultDataTable( schemaTable );
            this.resultDataSet.Tables.Add( this.resultDataTable );
            this.resultDataTable.Schema.TableName = string.Format( "Table{0}", this.resultDataSet.Tables.Count );
        }

        void IResultWriter.FirstRowReadBegin()
        {
            this.logResultWriter.FirstRowReadBegin();
        }

        void IResultWriter.FirstRowReadEnd()
        {
            this.logResultWriter.FirstRowReadEnd();
        }

        void IResultWriter.WriteRows( object[][] rows, int rowCount )
        {
            this.logResultWriter.WriteRows( rows, rowCount );

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                this.resultDataTable.Rows.Add( rows[ rowIndex ] );
            }
        }

        void IResultWriter.WriteTableEnd()
        {
            this.logResultWriter.WriteTableEnd();
        }

        void IResultWriter.WriteParameters( IDataParameterCollection parameters )
        {
            this.logResultWriter.WriteParameters( parameters );
        }

        private void WriteResultDataSet()
        {
            long ticks = Stopwatch.GetTimestamp();
            string message = string.Format( "Generating Excel file {0}...", this.fileName );
            this.addInfoMessage( new InfoMessage( LocalTime.Default.Now, InfoMessageSeverity.Verbose, message ) );

            var tables =
                from table in this.resultDataSet.Tables
                select table.Schema;

            this.xmlSpreadSheetWriter.WriteStyles( tables );

            foreach (var dataTable in this.resultDataSet.Tables)
            {
                this.xmlSpreadSheetWriter.WriteStartTable( dataTable.Schema );

                foreach (var dataRow in dataTable.Rows)
                {
                    this.xmlSpreadSheetWriter.WriteRow( dataRow );
                }

                this.xmlSpreadSheetWriter.WriteEndTable();
            }

            this.xmlSpreadSheetWriter.XmlWriter.Close();

            ticks = Stopwatch.GetTimestamp() - ticks;
            message = string.Format( "Excel file generated successfully {0} in seconds.", StopwatchTimeSpan.ToString( ticks, 3 ) );
            this.addInfoMessage( new InfoMessage( LocalTime.Default.Now, InfoMessageSeverity.Verbose, message ) );

            var processStartInfo = new ProcessStartInfo( this.fileName );
            Process.Start( processStartInfo );
        }

        void IResultWriter.End()
        {
            this.logResultWriter.End();
            if (this.resultDataSet != null)
            {
                Task.Factory.StartNew( this.WriteResultDataSet );
            }
        }

        #endregion

        private sealed class ResultDataSet
        {
            public List<ResultDataTable> Tables = new List<ResultDataTable>();
        }

        private sealed class ResultDataTable
        {
            private XmlSpreadsheetTable schema = new XmlSpreadsheetTable();
            public List<object[]> Rows = new List<object[]>();

            public ResultDataTable( DataTable schemaTable )
            {
                Contract.Requires( schemaTable != null );

                foreach (DataRow schemaRow in schemaTable.Rows)
                {
                    var source = new DataColumnSchema( schemaRow );
                    var target = new XmlSpreadsheetColumn
                    {
                        ColumnName = source.ColumnName
                    };

                    TypeCode typeCode = Type.GetTypeCode( source.DataType );

                    switch (typeCode)
                    {
                        case TypeCode.Boolean:
                            target.DataType = XmlSpreadsheetDataType.Boolean;
                            target.Convert = BooleanToXmlString;
                            break;

                        case TypeCode.DateTime:
                            target.DataType = XmlSpreadsheetDataType.DateTime;
                            target.NumberFormat = "General Date";
                            target.Convert = DateTimeToXmlString;
                            break;

                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Int16:
                        case TypeCode.Byte:
                            target.DataType = XmlSpreadsheetDataType.Number;
                            target.Convert = ObjectToXmlString;
                            break;

                        case TypeCode.Decimal:
                            target.DataType = XmlSpreadsheetDataType.Number;
                            target.Convert = DecimalToXmlString;
                            break;

                        case TypeCode.Double:
                            target.DataType = XmlSpreadsheetDataType.Number;
                            target.Convert = DoubleToXmlString;
                            break;

                        case TypeCode.String:
                            target.DataType = XmlSpreadsheetDataType.String;
                            target.Convert = ObjectToXmlString;
                            break;

                        default:
                            target.DataType = XmlSpreadsheetDataType.String;
                            target.Convert = ObjectToXmlString;
                            break;
                    }

                    // TODO
                    // target.Width =                     

                    this.schema.Columns.Add( target );
                }
            }

            public XmlSpreadsheetTable Schema
            {
                get
                {
                    return this.schema;
                }
            }

            private static string ObjectToXmlString( object value )
            {
                return value.ToString();
            }

            private static string BooleanToXmlString( object value )
            {
                return (bool) value ? "1" : "0";
            }

            private static string DateTimeToXmlString( object value )
            {
                DateTime dateTime;
                var dateTimeField = value as DateTimeField;
                if (dateTimeField != null)
                {
                    dateTime = dateTimeField.Value;
                }
                else
                {
                    string dateTimeString = value as string;
                    if (dateTimeString != null)
                    {
                        dateTime = DateTime.ParseExact( dateTimeString, new string[] { "yyyy-MM-dd", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None );
                    }
                    else
                    {
                        dateTime = (DateTime) value;
                    }
                }

                return XmlConvert.ToString( dateTime, XmlDateTimeSerializationMode.Unspecified );
            }

            private static string DecimalToXmlString( object value )
            {
                string xmlValue;
                var decimalField = value as DecimalField;
                if (decimalField != null)
                {
                    xmlValue = XmlConvert.ToString( decimalField.DecimalValue );
                }
                else
                {
                    xmlValue = XmlConvert.ToString( (decimal) value );
                }
                return xmlValue;
            }

            private static string DoubleToXmlString( object value )
            {
                var doubleField = value as DoubleField;
                string xmlValue = XmlConvert.ToString( doubleField.Value );
                return xmlValue;
            }
        }
    }
}