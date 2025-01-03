using System;
using System.Windows.Forms;
using DataCommander.Api.ResultWriter;

namespace DataCommander.Application.ResultWriter;

internal partial class DataViewPropertiesForm : Form
{
    private DataViewProperties? _properties;

    public DataViewPropertiesForm(DataViewProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        _properties = properties;
        InitializeComponent();

        rowFilterTextBox.Text = properties.RowFilter;
        sortTextBox.Text = properties.Sort;
    }

    private void okButton_Click(object? sender, EventArgs e)
    {
        _properties = new DataViewProperties(rowFilterTextBox.Text, sortTextBox.Text);
        DialogResult = DialogResult.OK;
    }
}