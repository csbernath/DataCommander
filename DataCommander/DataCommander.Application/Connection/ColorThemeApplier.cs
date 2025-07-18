using System.Drawing;
using System.Windows.Forms;
using DataCommander.Api;

namespace DataCommander.Application.Connection;

internal static class ColorThemeApplier
{
    public static void Apply(this ColorTheme colorTheme, DataGridView dataGridView)
    {
        var backColor = colorTheme.BackColor;
        if (backColor != null)
        {
            dataGridView.BackgroundColor = backColor.Value;
            dataGridView.BackColor = backColor.Value;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = backColor.Value;
            dataGridView.RowsDefaultCellStyle.BackColor = backColor.Value;
            dataGridView.RowHeadersDefaultCellStyle.BackColor = backColor.Value;
            dataGridView.RowHeadersDefaultCellStyle.ForeColor = backColor.Value;
        }
        else
            dataGridView.EnableHeadersVisualStyles = false;

        var foreColor = colorTheme.ForeColor;
        if (foreColor != null)
        {
            dataGridView.ForeColor = foreColor.Value;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = foreColor.Value;
            dataGridView.RowsDefaultCellStyle.ForeColor = foreColor.Value;
        }

        //dataGridView.EnableHeadersVisualStyles = true;
    }

    public static void Apply(this ColorTheme colorTheme, Control control)
    {
        // TODO
        // control.ForeColor = colorTheme.ForeColor;
        // control.BackColor = colorTheme.BackColor;
        //
        // foreach (Control childControl in control.Controls)
        //     colorTheme.Apply(childControl);
    }

    public static void Apply(this ColorTheme colorTheme, ToolStripItem toolStripItem)
    {
        // TODO
        // toolStripItem.ForeColor = colorTheme.ForeColor;
        // toolStripItem.BackColor = colorTheme.BackColor;
    }
}