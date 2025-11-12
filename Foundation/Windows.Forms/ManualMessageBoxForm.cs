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

        private void button1_Click(object sender, System.EventArgs e)
        {

        }

        private void label1_Click(object sender, System.EventArgs e)
        {

        }
    }
}