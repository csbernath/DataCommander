using System.Windows.Forms;
using DataCommander.Api;

namespace DataCommander.Application.Connection;

internal static class ColorThemeApplier
{
    extension(ColorTheme colorTheme)
    {
        public void Apply(DataGridView dataGridView)
        {
            var foreColor = colorTheme.ForeColor;
            if (foreColor != null)
            {
                dataGridView.ForeColor = foreColor.Value;
                dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = foreColor.Value;
                dataGridView.RowsDefaultCellStyle.ForeColor = foreColor.Value;
            }

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

            dataGridView.EnableHeadersVisualStyles = false;
        }

        public void Apply(Control control)
        {
            control.ForeColor = colorTheme.ForeColor!.Value;
            control.BackColor = colorTheme.BackColor!.Value;

            foreach (Control childControl in control.Controls)
                colorTheme.Apply(childControl);
        }

        public void Apply(ToolStripItem toolStripItem)
        {
            toolStripItem.ForeColor = colorTheme.ForeColor!.Value;
            toolStripItem.BackColor = colorTheme.BackColor!.Value;
        }
    }
}