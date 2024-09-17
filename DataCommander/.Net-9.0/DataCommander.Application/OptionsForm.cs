using System.Drawing;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Application.Connection;

namespace DataCommander.Application;

public partial class OptionsForm : Form
{
    private bool _darkColorTheme;
    private Font _font;

    public OptionsForm(bool darkColorTheme, Font font, ColorTheme? colorTheme)
    {
        InitializeComponent();

        _darkColorTheme = darkColorTheme;
        _font = font;

        colorThemeComboBox.SelectedIndex = _darkColorTheme ? 1 : 0;

        colorTheme?.Apply(this);
    }

    public bool DarkColorTheme => _darkColorTheme;
    public Font SelectedFont => _font;

    private void okButton_Click(object sender, System.EventArgs e)
    {
        _darkColorTheme = colorThemeComboBox.SelectedIndex != 0;
    }

    private void changeFontButton_Click(object sender, System.EventArgs e)
    {
        FontDialog fontDialog = new FontDialog
        {
            Font = _font
        };
        DialogResult dialogResult = fontDialog.ShowDialog();

        if (dialogResult == DialogResult.OK)
            _font = fontDialog.Font;
    }
}