using System.Windows.Forms;

namespace DataCommander.Providers
{
    public partial class OptionsForm : Form
    {
        private bool _darkColorTheme;

        public OptionsForm(bool darkColorTheme)
        {
            InitializeComponent();

            _darkColorTheme = darkColorTheme;
            colorThemeComboBox.SelectedIndex = _darkColorTheme ? 1 : 0;
        }

        public bool DarkColorTheme => _darkColorTheme;

        private void okButton_Click(object sender, System.EventArgs e)
        {
            _darkColorTheme = colorThemeComboBox.SelectedIndex != 0;
        }
    }
}