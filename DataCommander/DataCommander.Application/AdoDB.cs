using System;
using System.IO;
using System.Reflection;
using ADODB;
using Foundation.Text;

namespace DataCommander.Application;

public static class AdoDb
{
    [CLSCompliant(false)]
    public static RecordsetClass XmlToRecordset(string xml)
    {
        var stream = new StreamClass();
        stream.Open(Missing.Value, ConnectModeEnum.adModeUnknown, StreamOpenOptionsEnum.adOpenStreamUnspecified, null, null);
        stream.WriteText(xml, 0);
        stream.Position = 0;
        var recordset = new RecordsetClass();
        recordset.Open(stream, Missing.Value, CursorTypeEnum.adOpenUnspecified, LockTypeEnum.adLockUnspecified, 0);
        return recordset;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rs"></param>
    /// <param name="writer"></param>
    [CLSCompliant(false)]
    public static void WriteSchema(_Recordset rs, TextWriter writer)
    {
        var index = 1;
        var d = (double)rs.Fields.Count;
        d = Math.Log10(d);
        var colWidth = (int)d;
        colWidth++;

        foreach (Field field in rs.Fields)
        {
            var fieldType = field.Type;

            var line =
                StringHelper.FormatColumn(index.ToString(), colWidth, false) + ". " +
                StringHelper.FormatColumn(field.Name, 30, false);

            var fileTypeStr = Enum.Format(fieldType.GetType(), field.Type, "g");

            switch (fieldType)
            {
                case DataTypeEnum.adNumeric:
                    fileTypeStr += "(" + field.Precision + "," + field.NumericScale + ")";
                    break;

                case DataTypeEnum.adVarChar:
                    fileTypeStr += "(" + field.DefinedSize + ")";
                    break;

                default:
                    break;
            }

            if ((field.Attributes & (int)FieldAttributeEnum.adFldMayBeNull) != 0)
                fileTypeStr += " NULL";
            else
                fileTypeStr += " NOT NULL";

            line += StringHelper.FormatColumn(fileTypeStr, 30, false);

            writer.WriteLine(line);
            index++;
        }
    }

    public static void WriteSchema(object adodbRecordset, TextWriter writer) => WriteSchema((Recordset)adodbRecordset, writer);

    [CLSCompliant(false)]
    public static void WriteRows(_Recordset recordset, int maxRowCount, TextWriter writer)
    {
        var recordCount = recordset.RecordCount;
        writer.WriteLine("RecordCount: " + recordCount);

        if (!recordset.EOF)
        {
            var rsStr = recordset.GetString(StringFormatEnum.adClipString, maxRowCount, "\t", "\r\n", "<NULL>");
            writer.WriteLine(rsStr);
        }
    }

    public static void WriteRows(object adodbRecordset, TextWriter writer) => WriteRows((Recordset)adodbRecordset, int.MaxValue, writer);

    public static void Write(object adodbRecordset, int maxRowCount, TextWriter writer)
    {
        if (adodbRecordset != null)
        {
            var rs = (Recordset)adodbRecordset;
            WriteSchema(rs, writer);
            WriteRows(rs, maxRowCount, writer);
        }
    }

    public static void Write(object adodbRecordset, TextWriter writer)
    {
        if (adodbRecordset != null)
        {
            var rs = (Recordset)adodbRecordset;
            WriteSchema(rs, writer);
            WriteRows(rs, int.MaxValue, writer);
        }
    }
}