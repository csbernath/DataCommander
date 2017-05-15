namespace DataCommander.Providers.ResultWriter
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.IO;
    using System.Text;
    using Connection;
    using Foundation.Data.SqlClient;
    using Foundation.Diagnostics.MethodProfiler;
    using Foundation.Text;
    using Query;

    internal sealed class TextResultWriter : IResultWriter
    {
        private readonly IResultWriter logResultWriter;
        private readonly TextWriter textWriter;
        private readonly QueryForm queryForm;
        private int[] columnSize;
        private int rowIndex;
        private IProvider provider;

        public TextResultWriter(
            Action<InfoMessage> addInfoMessage,
            TextWriter textWriter,
            QueryForm queryForm)
        {
            this.logResultWriter = new LogResultWriter(addInfoMessage);
            this.textWriter = textWriter;
            this.queryForm = queryForm;
        }

        void IResultWriter.Begin(IProvider provider)
        {
            this.logResultWriter.Begin(provider);
            this.provider = provider;
        }

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
        {
            this.logResultWriter.BeforeExecuteReader(command);
        }

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
            this.logResultWriter.AfterExecuteReader(fieldCount);
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
            this.logResultWriter.AfterCloseReader(affectedRows);
        }

        private void Write(
            StringBuilder sb,
            string text,
            int width)
        {
            var length = width - text.Length;

            if (length > 0)
            {
                sb.Append(text);

                if (text.IndexOf(Environment.NewLine) < 0)
                {
                    sb.Append(' ', length);
                }
            }
            else
            {
                sb.Append(text.Substring(0, width));
            }
        }

        public void WriteTableBegin(DataTable schemaTable)
        {
            this.logResultWriter.WriteTableBegin(schemaTable);

            this.rowIndex = 0;

            if (schemaTable != null)
            {
                var columnNameColumn = schemaTable.Columns[SchemaTableColumn.ColumnName];
                var columnSizeColumn = schemaTable.Columns["ColumnSize"];
                var dataTypeColumn = schemaTable.Columns["DataType"];

                var fieldCount = schemaTable.Rows.Count;
                this.columnSize = new int[fieldCount];

                var sb = new StringBuilder();

                for (var i = 0; i < fieldCount; i++)
                {
                    var schemaRow = schemaTable.Rows[i];
                    var columnName = (string)schemaRow[columnNameColumn];
                    var type = (Type)schemaRow[dataTypeColumn];
                    var numOfBytes = (int)schemaRow[columnSizeColumn];

                    Type elementType;

                    if (type.IsArray)
                    {
                        elementType = type.GetElementType();

                        if (numOfBytes > 2048)
                        {
                            numOfBytes = 2048;
                        }
                    }
                    else
                    {
                        elementType = type;
                    }

                    var typeCode = Type.GetTypeCode(elementType);

                    int numOfChars;

                    switch (typeCode)
                    {
                        case TypeCode.Byte:
                            if (type.IsArray)
                            {
                                numOfChars = Math.Min(100, numOfBytes)*2 + 2;
                            }
                            else
                            {
                                numOfChars = 4;
                            }

                            break;

                        case TypeCode.Boolean:
                            numOfChars = 5;
                            break;

                        case TypeCode.Int16:
                            numOfChars = 5;
                            break;

                        case TypeCode.Int32:
                            numOfChars = 11;
                            break;

                        case TypeCode.Double:
                            numOfChars = double.MaxValue.ToString().Length;
                            break;

                        case TypeCode.Decimal:
                            //numOfChars = numOfBytes;
                            var precision = (short)schemaRow["NumericPrecision"];
                            var scale = schemaRow.Field<short>("NumericScale");
                            numOfChars = precision + 3;
                            break;

                        case TypeCode.DateTime:
                            numOfChars = 23;
                            break;

                        case TypeCode.String:
                            numOfChars = Math.Min(numOfBytes, 2048);
                            break;

                        default:
                            if (elementType == typeof (Guid))
                            {
                                numOfChars = Guid.Empty.ToString().Length;
                            }
                            else
                            {
                                numOfChars = numOfBytes;
                            }

                            break;
                    }

                    this.columnSize[i] = Math.Max(numOfChars, columnName.Length);
                    this.Write(sb, columnName, this.columnSize[i]);
                    sb.Append(' ');
                }

                this.textWriter.WriteLine(sb.ToString());

                if (fieldCount > 0)
                {
                    sb = new StringBuilder();
                    var last = fieldCount - 1;

                    for (var i = 0; i < last; i++)
                    {
                        sb.Append('-', this.columnSize[i]);
                        sb.Append(' ');
                    }

                    sb.Append('-', this.columnSize[last]);

                    this.textWriter.WriteLine(sb.ToString());
                }
            }
        }

        private static string StringValue(object value, int columnSize)
        {
            string stringValue = null;

            if (value == null)
            {
                stringValue = "null";
            }
            else if (value == DBNull.Value)
            {
                stringValue = "NULL";
            }
            else
            {
                var type = value.GetType();
                var typeCode = Type.GetTypeCode(type);
                var isArray = type.IsArray;

                if (isArray)
                {
                    if (typeCode == TypeCode.Byte)
                    {
                        var bytes = (byte[])value;
                        var sb = new StringBuilder();
                        sb.Append("0x");

                        for (var i = 0; i < bytes.Length; i++)
                        {
                            var s = bytes[i].ToString("x");

                            if (s.Length == 1)
                            {
                                s = "0" + s;
                            }

                            sb.Append(s);
                        }

                        stringValue = sb.ToString();
                    }
                }
                else
                {
                    switch (typeCode)
                    {
                        case TypeCode.Int32:
                            stringValue = value.ToString().PadLeft(columnSize);
                            break;

                        case TypeCode.DateTime:
                            var dateTime = (DateTime)value;
                            stringValue = dateTime.ToString("yyyy.MM.dd HH:mm:ss.fff");
                            break;

                        default:
                            stringValue = value.ToString();
                            stringValue = stringValue.Replace("\x00", null);
                            break;
                    }
                }
            }

            return stringValue;
        }

        public void FirstRowReadBegin()
        {
            this.logResultWriter.FirstRowReadBegin();
        }

        public void FirstRowReadEnd(string[] dataTypeNames)
        {
            this.logResultWriter.FirstRowReadEnd(dataTypeNames);
        }

        public void WriteRows(object[][] rows, int rowCount)
        {
            MethodProfiler.BeginMethod();
            this.logResultWriter.WriteRows(rows, rowCount);

            try
            {
                var sb = new StringBuilder();

                for (var i = 0; i < rowCount; i++)
                {
                    var row = rows[i];
                    var last = row.Length - 1;

                    for (var j = 0; j < last; j++)
                    {
                        this.Write(sb, StringValue(row[j], this.columnSize[j]), this.columnSize[j]);
                        sb.Append(' ');
                    }

                    sb.Append(StringValue(row[last], this.columnSize[last]));
                    sb.Append(Environment.NewLine);
                }

                this.rowIndex += rowCount;

                this.textWriter.Write(sb.ToString());
            }
            finally
            {
                MethodProfiler.EndMethod();
            }
        }

        public void WriteTableEnd()
        {
            this.logResultWriter.WriteTableEnd();
        }

        public static void WriteParameters(
            IDataParameterCollection parameters,
            TextWriter textWriter,
            QueryForm queryForm)
        {
            if (parameters.Count > 0)
            {
                var stringTable = new StringTable(4);

                foreach (IDataParameter parameter in parameters)
                {
                    var name = parameter.ParameterName;
                    var value = parameter.Value;
                    string valueString;

                    if (value == null)
                    {
                        valueString = "(null)";
                    }
                    else if (value == DBNull.Value)
                    {
                        valueString = "<NULL>";
                    }
                    else
                    {
                        switch (parameter.DbType)
                        {
                            case DbType.DateTime:
                                var dateTime = (DateTime)value;
                                valueString = dateTime.ToTSqlDateTime();
                                break;

                            default:
                                valueString = value.ToString();
                                break;
                        }
                    }

                    //    Type type = value.GetType();

                    //    if (type.IsArray)
                    //    {
                    //        Array array = (Array) value;
                    //        StringBuilder sb = new StringBuilder();

                    //        for (Int32 i = 0; i < array.Length; i++)
                    //        {
                    //            if (i > 0)
                    //            {
                    //                sb.Append( ',' );
                    //            }

                    //            sb.Append( array.GetValue(i));
                    //        }

                    //        valueString = sb.ToString();
                    //    }
                    //    else
                    //    {
                    //        string text = value as string;

                    //        if (text != null)
                    //        {
                    //            bool isXml = false;
                    //            XmlDocument xmlDocument = new XmlDocument();

                    //            try
                    //            {
                    //                xmlDocument.LoadXml( text );
                    //                isXml = true;
                    //            }
                    //            catch
                    //            {
                    //            }

                    //            if (isXml)
                    //            {
                    //                XmlAttribute xmlAttribute = xmlDocument.DocumentElement.Attributes[ "xmlns:rs" ];

                    //                if (xmlAttribute != null)
                    //                {
                    //                    valueString = "ADODB.Recordset (XML)";
                    //                    ADODB.Recordset rs = DataCommander.Foundation.Data.AdoDB.XmlToRecordset( text );
                    //                    DataTable dataTable = OleDBHelper.Convert( rs );
                    //                    dataTable.TableName = name;
                    //                    queryForm.Invoke( new QueryForm.ShowDataTableDelegate( queryForm.ShowDataTable ), new object[] { dataTable, TableStyle.DataGrid } );
                    //                }
                    //                else
                    //                {
                    //                    valueString = "XML";
                    //                    queryForm.Invoke( new QueryForm.ShowXmlDelegate( queryForm.ShowXml ), new object[] { name, text } );
                    //                }
                    //            }
                    //            else
                    //            {
                    //                valueString = text;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            ADODB.Recordset rs = value as ADODB.Recordset;

                    //            if (rs != null)
                    //            {
                    //                valueString = "ADODB.Recordset";
                    //            }
                    //            else
                    //            {
                    //                valueString = value.ToString();
                    //            }
                    //        }
                    //    }
                    //}

                    var row = stringTable.NewRow();
                    row[0] = parameter.Direction.ToString();
                    row[1] = name;
                    row[2] = parameter.DbType.ToString("G");
                    row[3] = valueString;
                    stringTable.Rows.Add(row);

                    //sb.Append(StringHelper.FormatColumn(parameter.DbType.ToString(),22));       
                }

                textWriter.WriteLine(stringTable);
            }
        }

        public void WriteParameters(IDataParameterCollection parameters)
        {

            WriteParameters(parameters, this.textWriter, this.queryForm);
        }

        public void WriteEnd()
        {
        }

        void IResultWriter.End()
        {
            this.logResultWriter.End();
        }
    }
}