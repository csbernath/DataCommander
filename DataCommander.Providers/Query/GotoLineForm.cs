namespace DataCommander.Providers.Query
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for GotoLineForm.
    /// </summary>
    public class GotoLineForm : Form
    {
        private Label _lineNumberLabel;
        private TextBox _lineNumberTextBox;
        private Button _okButton;
        private Button _cancelButton;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container _components = null;

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
            _lineNumberLabel.Text = $"Line number (1 - {maxLineLineNumber}):";
            _lineNumberTextBox.Text = currentLineNumber.ToString();
        }

        public int LineNumber
        {
            get
            {
                var s = _lineNumberTextBox.Text;
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._lineNumberLabel = new System.Windows.Forms.Label();
            this._lineNumberTextBox = new System.Windows.Forms.TextBox();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lineNumberLabel
            // 
            this._lineNumberLabel.Location = new System.Drawing.Point(8, 8);
            this._lineNumberLabel.Name = "_lineNumberLabel";
            this._lineNumberLabel.Size = new System.Drawing.Size(128, 16);
            this._lineNumberLabel.TabIndex = 0;
            this._lineNumberLabel.Text = "Line number (1 - {0}):";
            // 
            // lineNumberTextBox
            // 
            this._lineNumberTextBox.Location = new System.Drawing.Point(8, 24);
            this._lineNumberTextBox.Name = "_lineNumberTextBox";
            this._lineNumberTextBox.Size = new System.Drawing.Size(208, 20);
            this._lineNumberTextBox.TabIndex = 1;
            this._lineNumberTextBox.Text = "";
            // 
            // okButton
            // 
            this._okButton.Location = new System.Drawing.Point(56, 56);
            this._okButton.Name = "_okButton";
            this._okButton.TabIndex = 2;
            this._okButton.Text = "OK";
            this._okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(136, 56);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "Cancel";
            // 
            // GotoLineForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(224, 86);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._lineNumberTextBox);
            this.Controls.Add(this._lineNumberLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GotoLineForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Go To Line";
            this.ResumeLayout(false);

        }
        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                var s = _lineNumberTextBox.Text;
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
}