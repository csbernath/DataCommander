using System.Data;
using System.Drawing;

namespace DataCommander.Application.Connection;

internal sealed class DataRowSelector(DataColumn column, Graphics graphics, Font font)
{
    public float GetWidth(DataRow row)
    {
        string? s = row[column].ToString();
        int length = s.Length;
        float width = length <= 256 ? graphics.MeasureString(s, font).Width : 100;
        return width;
    }
}