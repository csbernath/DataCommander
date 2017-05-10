namespace DataCommander.Providers.Query
{
    using System.Drawing;

    public sealed class ColorTheme
    {
        public ColorTheme(Color backColor, Color foreColor)
        {
            BackColor = backColor;
            ForeColor = foreColor;
        }

        public Color BackColor { get; }
        public Color ForeColor { get; }
    }
}