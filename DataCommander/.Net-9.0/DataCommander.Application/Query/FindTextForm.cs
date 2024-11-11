using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DataCommander.Application.Query;

public class FindTextForm : Form
{
    private ComboBox? _cbText;
    private Button? _btnOk;
    private Button? _btnCancel;
    private Label? _label1;
    private CheckBox _cbMatchCase;
    private CheckBox _cbMatchWholeWord;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private readonly Container? _components = null;

    public FindTextForm()
    {
        //
        // Required for Windows Form Designer support
        //
        InitializeComponent();

        //
        // TODO: Add any constructor code after InitializeComponent call
        //
    }

    public string FindText
    {
        get => _cbText!.Text;
        set => _cbText!.Text = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public RichTextBoxFinds RichTextBoxFinds
    {
        get
        {
            var richTextBoxFinds = RichTextBoxFinds.None;

            if (_cbMatchCase.Checked)
            {
                richTextBoxFinds |= RichTextBoxFinds.MatchCase;
            }

            if (_cbMatchWholeWord.Checked)
            {
                richTextBoxFinds |= RichTextBoxFinds.WholeWord;
            }

            return richTextBoxFinds;
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
        _cbText = new ComboBox();
        _btnOk = new Button();
        _btnCancel = new Button();
        _label1 = new Label();
        _cbMatchCase = new CheckBox();
        _cbMatchWholeWord = new CheckBox();
        SuspendLayout();
        // 
        // cbText
        // 
        _cbText.Location = new System.Drawing.Point(72, 8);
        _cbText.Name = "_cbText";
        _cbText.Size = new System.Drawing.Size(216, 21);
        _cbText.TabIndex = 0;
        // 
        // btnOK
        // 
        _btnOk.DialogResult = DialogResult.OK;
        _btnOk.Location = new System.Drawing.Point(64, 88);
        _btnOk.Name = "_btnOk";
        _btnOk.TabIndex = 1;
        _btnOk.Text = "OK";
        _btnOk.Click += new EventHandler(btnOK_Click);
        // 
        // btnCancel
        // 
        _btnCancel.DialogResult = DialogResult.Cancel;
        _btnCancel.Location = new System.Drawing.Point(152, 88);
        _btnCancel.Name = "_btnCancel";
        _btnCancel.TabIndex = 2;
        _btnCancel.Text = "Cancel";
        // 
        // label1
        // 
        _label1.Location = new System.Drawing.Point(8, 12);
        _label1.Name = "_label1";
        _label1.Size = new System.Drawing.Size(56, 16);
        _label1.TabIndex = 3;
        _label1.Text = "Find what:";
        // 
        // cbMatchCase
        // 
        _cbMatchCase.Location = new System.Drawing.Point(8, 40);
        _cbMatchCase.Name = "_cbMatchCase";
        _cbMatchCase.Size = new System.Drawing.Size(104, 16);
        _cbMatchCase.TabIndex = 4;
        _cbMatchCase.Text = "Match &case";
        // 
        // cbMatchWholeWord
        // 
        _cbMatchWholeWord.Location = new System.Drawing.Point(8, 64);
        _cbMatchWholeWord.Name = "_cbMatchWholeWord";
        _cbMatchWholeWord.Size = new System.Drawing.Size(122, 19);
        _cbMatchWholeWord.TabIndex = 5;
        _cbMatchWholeWord.Text = "Match &whole word";
        // 
        // FindTextForm
        // 
        AcceptButton = _btnOk;
        AutoScaleBaseSize = new System.Drawing.Size(5, 14);
        CancelButton = _btnCancel;
        ClientSize = new System.Drawing.Size(292, 114);
        Controls.AddRange([
            _cbMatchWholeWord,
            _cbMatchCase,
            _label1,
            _btnCancel,
            _btnOk,
            _cbText]);
        Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((Byte)(238)));
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Name = "FindTextForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Find";
        Closing += new CancelEventHandler(FindTextForm_Closing);
        ResumeLayout(false);

    }

    private void btnOK_Click(object? sender, EventArgs e)
    {
        var text = _cbText!.Text;

        if (text.Length > 0)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                var i = _cbText.FindStringExact(text);

                if (i < 0)
                {
                    _cbText.Items.Insert(0, text);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }

    private void FindTextForm_Closing(object? sender, CancelEventArgs e) => _cbText!.Focus();
}