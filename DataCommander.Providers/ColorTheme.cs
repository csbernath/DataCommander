namespace DataCommander.Providers
{
    using System.Drawing;

    public sealed class ColorTheme
    {
        public ColorTheme(Color foreColor, Color backColor, Color execKeyWordColor, Color sqlKeyWordColor, Color providerKeyWordColor)
        {
            ForeColor = foreColor;
            BackColor = backColor;
            ExecKeyWordColor = execKeyWordColor;
            SqlKeyWordColor = sqlKeyWordColor;
            ProviderKeyWordColor = providerKeyWordColor;
        }

        public readonly Color ForeColor;
        public readonly Color BackColor;
        public readonly Color ExecKeyWordColor;
        public readonly Color SqlKeyWordColor;
        public readonly Color ProviderKeyWordColor;
    }
}