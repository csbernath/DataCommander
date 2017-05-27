using System.Data;
using System.Drawing;

namespace DataCommander.Providers.Connection
{
    internal sealed class DataRowSelector
    {
        private readonly DataColumn _column;
        private readonly Graphics _graphics;
        private readonly Font _font;

        public DataRowSelector(DataColumn column, Graphics graphics, Font font)
        {
            _column = column;
            _graphics = graphics;
            _font = font;
        }

        public float GetWidth(DataRow row)
        {
            var s = row[_column].ToString();
            var length = s.Length;
            float width;

            if (length <= 256)
                width = _graphics.MeasureString(s, _font).Width;
            else
                width = 100;

            return width;
        }
    }
}