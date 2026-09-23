using System;
using System.Drawing;

namespace DataCommander.Application.Query;

public static class ColorExtensions
{
    public static Color GetReadableForeColor(this Color backColor)
    {
        // Calculate contrast ratios against white and black
        double contrastWithWhite = GetContrastRatio(backColor, Color.White);
        double contrastWithBlack = GetContrastRatio(backColor, Color.Black);

        return contrastWithWhite >= contrastWithBlack ? Color.White : Color.Black;
    }

    private static double GetContrastRatio(Color c1, Color c2)
    {
        double l1 = GetRelativeLuminance(c1);
        double l2 = GetRelativeLuminance(c2);

        double lighter = Math.Max(l1, l2);
        double darker = Math.Min(l1, l2);

        return (lighter + 0.05) / (darker + 0.05);
    }

    private static double GetRelativeLuminance(Color c)
    {
        double r = TransformComponent(c.R / 255.0);
        double g = TransformComponent(c.G / 255.0);
        double b = TransformComponent(c.B / 255.0);

        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    private static double TransformComponent(double colorVal)
    {
        return colorVal <= 0.03928
            ? colorVal / 12.92
            : Math.Pow((colorVal + 0.055) / 1.055, 2.4);
    }
}