using System.Windows.Forms;
using DataCommander.Api;

namespace DataCommander.Application.Connection;

internal static class ColorThemeApplier
{
    public static void Apply(this ColorTheme colorTheme, DataGridView dataGridView)
    {
        dataGridView.BackgroundColor = colorTheme.BackColor;
        dataGridView.BackColor = colorTheme.BackColor;
        dataGridView.ForeColor = colorTheme.ForeColor;

        dataGridView.EnableHeadersVisualStyles = true;
        dataGridView.ColumnHeadersDefaultCellStyle.BackColor = colorTheme.BackColor;
        dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = colorTheme.ForeColor;
        dataGridView.RowsDefaultCellStyle.BackColor = colorTheme.BackColor;
        dataGridView.RowsDefaultCellStyle.ForeColor = colorTheme.ForeColor;
        dataGridView.RowHeadersDefaultCellStyle.BackColor = colorTheme.BackColor;
        dataGridView.RowHeadersDefaultCellStyle.ForeColor = colorTheme.BackColor;
    }

    public static void Apply(this ColorTheme colorTheme, Control control)
    {
        control.ForeColor = colorTheme.ForeColor;
        control.BackColor = colorTheme.BackColor;

        foreach (Control childControl in control.Controls)
            colorTheme.Apply(childControl);
    }
}