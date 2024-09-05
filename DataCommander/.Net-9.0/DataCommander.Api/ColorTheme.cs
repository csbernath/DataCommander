using System.Drawing;

namespace DataCommander.Api;

public sealed class ColorTheme(Color foreColor, Color backColor, Color execKeyWordColor, Color sqlKeyWordColor, Color providerKeyWordColor)
{
    public readonly Color ForeColor = foreColor;
    public readonly Color BackColor = backColor;
    public readonly Color ExecKeyWordColor = execKeyWordColor;
    public readonly Color SqlKeyWordColor = sqlKeyWordColor;
    public readonly Color ProviderKeyWordColor = providerKeyWordColor;
}