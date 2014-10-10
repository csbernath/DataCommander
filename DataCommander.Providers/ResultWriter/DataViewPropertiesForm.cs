namespace DataCommander.Providers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;

    internal partial class DataViewPropertiesForm : Form
    {
        private readonly DataViewProperties properties = new DataViewProperties();

        public DataViewPropertiesForm(DataViewProperties properties)
        {
            Contract.Requires(properties != null);

            this.properties = properties;
            this.InitializeComponent();

            this.rowFilterTextBox.Text = properties.RowFilter;
            this.sortTextBox.Text = properties.Sort;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.properties.RowFilter = this.rowFilterTextBox.Text;
            this.properties.Sort = this.sortTextBox.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}