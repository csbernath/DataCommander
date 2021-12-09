﻿using System;
using System.Windows.Forms;
using DataCommander.Providers2.ResultWriter;
using Foundation.Assertions;

namespace DataCommander.Providers.ResultWriter;

internal partial class DataViewPropertiesForm : Form
{
    private readonly DataViewProperties _properties = new();

    public DataViewPropertiesForm(DataViewProperties properties)
    {
        Assert.IsNotNull(properties);

        _properties = properties;
        InitializeComponent();

        rowFilterTextBox.Text = properties.RowFilter;
        sortTextBox.Text = properties.Sort;
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        _properties.RowFilter = rowFilterTextBox.Text;
        _properties.Sort = sortTextBox.Text;
        DialogResult = DialogResult.OK;
    }
}