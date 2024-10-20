using System.Data;
using System.Drawing;

namespace DataCommander.Application.Connection;

internal sealed class DataRowSelector(DataColumn column, Graphics graphics, Font font)
{
    public float GetWidth(DataRow row)
    {
        var s = row[column].ToString()!;
        var length = s.Length;
        var width = length <= 256 ? graphics.MeasureString(s, font).Width : 100;
        return width;
    }
}