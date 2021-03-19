﻿using System.Drawing;
using System.Windows.Forms;

namespace DataCommander.Providers
{
    public partial class OptionsForm : Form
    {
        private bool _darkColorTheme;
        private Font _font;

        public OptionsForm(bool darkColorTheme, Font font)
        {
            InitializeComponent();

            _darkColorTheme = darkColorTheme;
            _font = font;

            colorThemeComboBox.SelectedIndex = _darkColorTheme ? 1 : 0;
        }

        public bool DarkColorTheme => _darkColorTheme;
        public Font SelectedFont => _font;

        private void okButton_Click(object sender, System.EventArgs e)
        {
            _darkColorTheme = colorThemeComboBox.SelectedIndex != 0;
        }

        private void changeFontButton_Click(object sender, System.EventArgs e)
        {
            var fontDialog = new FontDialog();
            fontDialog.Font = _font;
            var dialogResult = fontDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
                _font = fontDialog.Font;
        }
    }
}