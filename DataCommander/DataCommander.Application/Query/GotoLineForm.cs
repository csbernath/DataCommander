using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DataCommander.Application.Query;

/// <summary>
/// Summary description for GotoLineForm.
/// </summary>
public class GotoLineForm : Form
{
    private Label? _lineNumberLabel;
    private TextBox? _lineNumberTextBox;
    private Button? _okButton;
    private Button? _cancelButton;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private readonly Container? _components = null;

    public GotoLineForm()
    {
        //
        // Required for Windows Form Designer support
        //
        InitializeComponent();

        //
        // TODO: Add any constructor code after InitializeComponent call
        //
    }

    public void Init(int currentLineNumber, int maxLineLineNumber)
    {
        _maxLineLineNumber = maxLineLineNumber;
        _lineNumberLabel!.Text = $"Line number (1 - {maxLineLineNumber}):";
        _lineNumberTextBox!.Text = currentLineNumber.ToString();
    }

    public int LineNumber
    {
        get
        {
            var s = _lineNumberTextBox!.Text;
            var lineNumber = int.Parse(s);
            return lineNumber;
        }
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_components != null)
            {
                _components.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        _lineNumberLabel = new Label();
        _lineNumberTextBox = new TextBox();
        _okButton = new Button();
        _cancelButton = new Button();
        SuspendLayout();
        // 
        // lineNumberLabel
        // 
        _lineNumberLabel.Location = new System.Drawing.Point(8, 8);
        _lineNumberLabel.Name = "_lineNumberLabel";
        _lineNumberLabel.Size = new System.Drawing.Size(128, 16);
        _lineNumberLabel.TabIndex = 0;
        _lineNumberLabel.Text = "Line number (1 - {0}):";
        // 
        // lineNumberTextBox
        // 
        _lineNumberTextBox.Location = new System.Drawing.Point(8, 24);
        _lineNumberTextBox.Name = "_lineNumberTextBox";
        _lineNumberTextBox.Size = new System.Drawing.Size(208, 20);
        _lineNumberTextBox.TabIndex = 1;
        _lineNumberTextBox.Text = "";
        // 
        // okButton
        // 
        _okButton.Location = new System.Drawing.Point(56, 56);
        _okButton.Name = "_okButton";
        _okButton.TabIndex = 2;
        _okButton.Text = "OK";
        _okButton.Click += new EventHandler(okButton_Click);
        // 
        // cancelButton
        // 
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Location = new System.Drawing.Point(136, 56);
        _cancelButton.Name = "_cancelButton";
        _cancelButton.TabIndex = 3;
        _cancelButton.Text = "Cancel";
        // 
        // GotoLineForm
        // 
        AcceptButton = _okButton;
        AutoScaleBaseSize = new System.Drawing.Size(5, 13);
        CancelButton = _cancelButton;
        ClientSize = new System.Drawing.Size(224, 86);
        Controls.Add(_cancelButton);
        Controls.Add(_okButton);
        Controls.Add(_lineNumberTextBox);
        Controls.Add(_lineNumberLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "GotoLineForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Go To Line";
        ResumeLayout(false);

    }

    private void okButton_Click(object? sender, EventArgs e)
    {
        try
        {
            var s = _lineNumberTextBox!.Text;
            var lineNumber = int.Parse(s);

            if (lineNumber >= 1 && lineNumber <= _maxLineLineNumber)
            {
                DialogResult = DialogResult.OK;
            }
        }
        catch
        {
        }
    }

    private int _maxLineLineNumber;
}