namespace DataCommander.Providers.Query
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for FindForm.
    /// </summary>
    public class FindTextForm : Form
    {
        private ComboBox cbText;
        private Button btnOK;
        private Button btnCancel;
        private Label label1;
        private CheckBox cbMatchCase;
        private CheckBox cbMatchWholeWord;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        /// <summary>
        /// 
        /// </summary>
        public FindTextForm()
        {
            //
            // Required for Windows Form Designer support
            //
            this.InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// 
        /// </summary>
        public string FindText
        {
            get => this.cbText.Text;

            set => this.cbText.Text = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public RichTextBoxFinds RichTextBoxFinds
        {
            get
            {
                var richTextBoxFinds = RichTextBoxFinds.None;

                if (this.cbMatchCase.Checked)
                {
                    richTextBoxFinds |= RichTextBoxFinds.MatchCase;
                }

                if (this.cbMatchWholeWord.Checked)
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
                if (this.components != null)
                {
                    this.components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbText = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbMatchCase = new System.Windows.Forms.CheckBox();
            this.cbMatchWholeWord = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbText
            // 
            this.cbText.Location = new System.Drawing.Point(72, 8);
            this.cbText.Name = "cbText";
            this.cbText.Size = new System.Drawing.Size(216, 21);
            this.cbText.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(64, 88);
            this.btnOK.Name = "btnOK";
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(152, 88);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Find what:";
            // 
            // cbMatchCase
            // 
            this.cbMatchCase.Location = new System.Drawing.Point(8, 40);
            this.cbMatchCase.Name = "cbMatchCase";
            this.cbMatchCase.Size = new System.Drawing.Size(104, 16);
            this.cbMatchCase.TabIndex = 4;
            this.cbMatchCase.Text = "Match &case";
            // 
            // cbMatchWholeWord
            // 
            this.cbMatchWholeWord.Location = new System.Drawing.Point(8, 64);
            this.cbMatchWholeWord.Name = "cbMatchWholeWord";
            this.cbMatchWholeWord.Size = new System.Drawing.Size(122, 19);
            this.cbMatchWholeWord.TabIndex = 5;
            this.cbMatchWholeWord.Text = "Match &whole word";
            // 
            // FindTextForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 114);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.cbMatchWholeWord,
                                                                  this.cbMatchCase,
                                                                  this.label1,
                                                                  this.btnCancel,
                                                                  this.btnOK,
                                                                  this.cbText});
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FindTextForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FindTextForm_Closing);
            this.ResumeLayout(false);

        }
        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            var text = this.cbText.Text;

            if (text.Length > 0)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    var i = this.cbText.FindStringExact(text);

                    if (i < 0)
                    {
                        this.cbText.Items.Insert(0, text);
                    }
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void FindTextForm_Closing(object sender, CancelEventArgs e)
        {
            this.cbText.Focus();
        }
    }
}