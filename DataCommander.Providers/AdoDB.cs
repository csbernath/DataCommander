namespace DataCommander.Foundation.Data
{
    using System;
    using System.IO;
    using System.Reflection;
    using DataCommander.Foundation.Text;

    /// <summary>
    /// 
    /// </summary>
    public static class AdoDB
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        [CLSCompliant( false )]
        public static ADODB.RecordsetClass XmlToRecordset( string xml )
        {
            var stream = new ADODB.StreamClass();

            stream.Open(
                Missing.Value,
                ADODB.ConnectModeEnum.adModeUnknown,
                ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified,
                null, null );

            stream.WriteText( xml, 0 );
            stream.Position = 0;
            var rs = new ADODB.RecordsetClass();

            rs.Open(
                stream, Missing.Value,
                ADODB.CursorTypeEnum.adOpenUnspecified,
                ADODB.LockTypeEnum.adLockUnspecified, 0 );

            return rs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="writer"></param>
        [CLSCompliant( false )]
        public static void WriteSchema( ADODB._Recordset rs, TextWriter writer )
        {
            int index = 1;
            double d = (double) rs.Fields.Count;
            d = Math.Log10( d );
            int colWidth = (int) d;
            colWidth++;

            foreach (ADODB.Field field in rs.Fields)
            {
                ADODB.DataTypeEnum fieldType = field.Type;

                string line =
                    StringHelper.FormatColumn( index.ToString(), colWidth, false ) + ". " +
                    StringHelper.FormatColumn( field.Name, 30, false );

                string fileTypeStr = Enum.Format( fieldType.GetType(), field.Type, "g" );

                switch (fieldType)
                {
                    case ADODB.DataTypeEnum.adNumeric:
                        fileTypeStr += "(" + field.Precision + "," + field.NumericScale + ")";
                        break;

                    case ADODB.DataTypeEnum.adVarChar:
                        fileTypeStr += "(" + field.DefinedSize + ")";
                        break;

                    default:
                        break;
                }

                if ((field.Attributes & (int) ADODB.FieldAttributeEnum.adFldMayBeNull) != 0)
                {
                    fileTypeStr += " NULL";
                }
                else
                {
                    fileTypeStr += " NOT NULL";
                }

                line += StringHelper.FormatColumn( fileTypeStr, 30, false );

                writer.WriteLine( line );
                index++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ADODBRecordset"></param>
        /// <param name="writer"></param>
        public static void WriteSchema( object ADODBRecordset, TextWriter writer )
        {
            WriteSchema( (ADODB.Recordset) ADODBRecordset, writer );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="maxRowCount"></param>
        /// <param name="writer"></param>
        [CLSCompliant( false )]
        public static void WriteRows(
            ADODB._Recordset rs,
            int maxRowCount,
            TextWriter writer )
        {
            int recordCount = rs.RecordCount;
            writer.WriteLine( "RecordCount: " + recordCount );

            if (!rs.EOF)
            {
                string rsStr = rs.GetString( ADODB.StringFormatEnum.adClipString, maxRowCount, "\t", "\r\n", "<NULL>" );
                writer.WriteLine( rsStr );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ADODBRecordset"></param>
        /// <param name="writer"></param>
        public static void WriteRows( object ADODBRecordset, TextWriter writer )
        {
            WriteRows( (ADODB.Recordset) ADODBRecordset, Int32.MaxValue, writer );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ADODBRecordset"></param>
        /// <param name="maxRowCount"></param>
        /// <param name="writer"></param>
        public static void Write(
            object ADODBRecordset,
            int maxRowCount,
            TextWriter writer )
        {
            if (ADODBRecordset != null)
            {
                ADODB.Recordset rs = (ADODB.Recordset) ADODBRecordset;
                WriteSchema( rs, writer );
                WriteRows( rs, maxRowCount, writer );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ADODBRecordset"></param>
        /// <param name="writer"></param>
        public static void Write(
            object ADODBRecordset,
            TextWriter writer )
        {
            if (ADODBRecordset != null)
            {
                ADODB.Recordset rs = (ADODB.Recordset) ADODBRecordset;
                WriteSchema( rs, writer );
                WriteRows( rs, Int32.MaxValue, writer );
            }
        }
    }
}