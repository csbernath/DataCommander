using System;
using System.ComponentModel;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Api.Query;

namespace DataCommander.Application.Query;

internal sealed class CompletionForm : Form
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private readonly Container? _components = null;

    private readonly QueryForm _queryForm;
    private EventHandler<ItemSelectedEventArgs>? _itemSelectedEvent;

    public CompletionForm(QueryForm queryForm)
    {
        //
        // Required for Windows Form Designer support
        //
        InitializeComponent();

        //
        // TODO: Add any constructor code after InitializeComponent call
        //
        _queryForm = queryForm;
    }

    public event EventHandler<ItemSelectedEventArgs> ItemSelected
    {
        add => _itemSelectedEvent += value;

        remove => _itemSelectedEvent -= value;
    }

    public void Initialize(QueryTextBox textBox, GetCompletionResult result, ColorTheme colorTheme)
    {
        var listBox = new MemberListBox(this, textBox, colorTheme);
        listBox.Initialize(result);
        listBox.Dock = DockStyle.Fill;

        Controls.Add(listBox);

        var charIndex = textBox.RichTextBox.SelectionStart;
        var pos = textBox.RichTextBox.GetPositionFromCharIndex(charIndex);
        var location = textBox.RichTextBox.PointToScreen(pos);
        location.Y += 20;
        Location = location;
    }

    public void SelectItem(int startIndex, int length, IObjectName objectName)
    {
        if (_itemSelectedEvent != null)
            _itemSelectedEvent(this, new ItemSelectedEventArgs(startIndex, length, objectName));
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            if (_components != null)
                _components.Dispose();

        base.Dispose(disposing);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        SuspendLayout();
        // 
        // CompletionForm
        // 
        AutoScaleBaseSize = new System.Drawing.Size(5, 13);
        ClientSize = new System.Drawing.Size(400, 140);
        ControlBox = false;
        FormBorderStyle = FormBorderStyle.None;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "CompletionForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        ResumeLayout(false);

    }
}