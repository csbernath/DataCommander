using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Foundation.Windows.Forms
{
    public partial class ManualMessageBoxForm : Form
    {
        public ManualMessageBoxForm(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon messageBoxIcon, MessageBoxDefaultButton defaultButton)
        {
            InitializeComponent();

            if (owner is Form ownerForm)
                Owner = ownerForm;

            Text = caption;

            var icon = MessageBoxBuilder.GetIcon(messageBoxIcon);
            pictureBox.Image = icon.ToBitmap();
        }
    }
}