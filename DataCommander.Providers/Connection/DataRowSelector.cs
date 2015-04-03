namespace DataCommander.Providers
{
    using System.Data;
    using System.Drawing;

    internal sealed class DataRowSelector
    {
        private readonly DataColumn column;
        private readonly Graphics graphics;
        private readonly Font font;

        public DataRowSelector(DataColumn column, Graphics graphics, Font font)
        {
            this.column = column;
            this.graphics = graphics;
            this.font = font;
        }

        public float GetWidth(DataRow row)
        {
            string s = row[this.column].ToString();
            int length = s.Length;
            float width;

            if (length <= 256)
            {
                width = this.graphics.MeasureString(s, this.font).Width;
            }
            else
            {
                width = 100;
            }

            return width;
        }
    }
}