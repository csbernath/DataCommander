using System;
using System.Reflection;
using ADODB;

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
}