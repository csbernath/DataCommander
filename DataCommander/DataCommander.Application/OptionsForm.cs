using System.Drawing;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Application.Connection;

namespace DataCommander.Application;

public partial class OptionsForm : Form
{
    private SystemColorMode _colorMode;
    private bool _initializeApplicationConfiguration;
    private Font _font;

    public OptionsForm(SystemColorMode colorMode, bool initializeApplicationConfiguration, Font font)
    {
        _colorMode = colorMode;
        _initializeApplicationConfiguration = initializeApplicationConfiguration;
        _font = font;
        
        InitializeComponent();

        // colorThemeComboBox.SelectedIndex = _colorMode ? 1 : 0;
        // colorTheme?.Apply(this);
    }

    public SystemColorMode ColorMode => _colorMode;
    public bool InitializeApplicationConfiguration => _initializeApplicationConfiguration;
    public Font SelectedFont => _font;

    private void okButton_Click(object? sender, System.EventArgs e)
    {
        // _darkColorTheme = colorThemeComboBox.SelectedIndex != 0;
    }

    private void changeFontButton_Click(object? sender, System.EventArgs e)
    {
        var fontDialog = new FontDialog
        {
            Font = _font
        };
        var dialogResult = fontDialog.ShowDialog();

        if (dialogResult == DialogResult.OK)
            _font = fontDialog.Font;
    }
}