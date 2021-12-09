using System.Drawing;

namespace DataCommander.Providers;

public sealed class ColorTheme
{
    public readonly Color ForeColor;
    public readonly Color BackColor;
    public readonly Color ExecKeyWordColor;
    public readonly Color SqlKeyWordColor;
    public readonly Color ProviderKeyWordColor;

    public ColorTheme(Color foreColor, Color backColor, Color execKeyWordColor, Color sqlKeyWordColor, Color providerKeyWordColor)
    {
        ForeColor = foreColor;
        BackColor = backColor;
        ExecKeyWordColor = execKeyWordColor;
        SqlKeyWordColor = sqlKeyWordColor;
        ProviderKeyWordColor = providerKeyWordColor;
    }
}