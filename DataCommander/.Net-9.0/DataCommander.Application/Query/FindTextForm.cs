using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DataCommander.Application.Query;

/// <summary>
/// Summary description for FindForm.
/// </summary>
public class FindTextForm : Form
{
    private ComboBox _cbText;
    private Button _btnOk;
    private Button _btnCancel;
    private Label _label1;
    private CheckBox _cbMatchCase;
    private CheckBox _cbMatchWholeWord;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private readonly Container _components = null;

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    public string FindText
    {
        get => _cbText.Text;

        set => _cbText.Text = value;
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
        this._cbText = new System.Windows.Forms.ComboBox();
        this._btnOk = new System.Windows.Forms.Button();
        this._btnCancel = new System.Windows.Forms.Button();
        this._label1 = new System.Windows.Forms.Label();
        this._cbMatchCase = new System.Windows.Forms.CheckBox();
        this._cbMatchWholeWord = new System.Windows.Forms.CheckBox();
        this.SuspendLayout();
        // 
        // cbText
        // 
        this._cbText.Location = new System.Drawing.Point(72, 8);
        this._cbText.Name = "_cbText";
        this._cbText.Size = new System.Drawing.Size(216, 21);
        this._cbText.TabIndex = 0;
        // 
        // btnOK
        // 
        this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
        this._btnOk.Location = new System.Drawing.Point(64, 88);
        this._btnOk.Name = "_btnOk";
        this._btnOk.TabIndex = 1;
        this._btnOk.Text = "OK";
        this._btnOk.Click += new System.EventHandler(this.btnOK_Click);
        // 
        // btnCancel
        // 
        this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this._btnCancel.Location = new System.Drawing.Point(152, 88);
        this._btnCancel.Name = "_btnCancel";
        this._btnCancel.TabIndex = 2;
        this._btnCancel.Text = "Cancel";
        // 
        // label1
        // 
        this._label1.Location = new System.Drawing.Point(8, 12);
        this._label1.Name = "_label1";
        this._label1.Size = new System.Drawing.Size(56, 16);
        this._label1.TabIndex = 3;
        this._label1.Text = "Find what:";
        // 
        // cbMatchCase
        // 
        this._cbMatchCase.Location = new System.Drawing.Point(8, 40);
        this._cbMatchCase.Name = "_cbMatchCase";
        this._cbMatchCase.Size = new System.Drawing.Size(104, 16);
        this._cbMatchCase.TabIndex = 4;
        this._cbMatchCase.Text = "Match &case";
        // 
        // cbMatchWholeWord
        // 
        this._cbMatchWholeWord.Location = new System.Drawing.Point(8, 64);
        this._cbMatchWholeWord.Name = "_cbMatchWholeWord";
        this._cbMatchWholeWord.Size = new System.Drawing.Size(122, 19);
        this._cbMatchWholeWord.TabIndex = 5;
        this._cbMatchWholeWord.Text = "Match &whole word";
        // 
        // FindTextForm
        // 
        this.AcceptButton = this._btnOk;
        this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
        this.CancelButton = this._btnCancel;
        this.ClientSize = new System.Drawing.Size(292, 114);
        this.Controls.AddRange([
            this._cbMatchWholeWord,
            this._cbMatchCase,
            this._label1,
            this._btnCancel,
            this._btnOk,
            this._cbText]);
        this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(238)));
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
        this.Name = "FindTextForm";
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Find";
        this.Closing += new System.ComponentModel.CancelEventHandler(this.FindTextForm_Closing);
        this.ResumeLayout(false);

    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        var text = _cbText.Text;

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

    private void FindTextForm_Closing(object sender, CancelEventArgs e) => _cbText.Focus();
}