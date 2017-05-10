namespace DataCommander.Providers.Query
{
    using System.Drawing;

    public sealed class ColorTheme
    {
        public ColorTheme(Color backColor, Color foreColor, Color execKeyWordColor, Color sqlKeyWordColor, Color providerKeyWordColor)
        {
            BackColor = backColor;
            ForeColor = foreColor;
            ExecKeyWordColor = execKeyWordColor;
            SqlKeyWordColor = sqlKeyWordColor;
            ProviderKeyWordColor = providerKeyWordColor;
        }

        public Color BackColor { get; }
        public Color ForeColor { get; }
        public Color ExecKeyWordColor { get; }
        public Color SqlKeyWordColor { get; }
        public Color ProviderKeyWordColor { get; }
    }
}