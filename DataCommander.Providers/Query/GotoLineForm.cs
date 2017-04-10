namespace DataCommander
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for GotoLineForm.
    /// </summary>
    public class GotoLineForm : Form
    {
        private Label lineNumberLabel;
        private TextBox lineNumberTextBox;
        private Button okButton;
        private Button cancelButton;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public GotoLineForm()
        {
            //
            // Required for Windows Form Designer support
            //
            this.InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public void Init(int currentLineNumber, int maxLineLineNumber)
        {
            this.maxLineLineNumber = maxLineLineNumber;
            this.lineNumberLabel.Text = $"Line number (1 - {maxLineLineNumber}):";
            this.lineNumberTextBox.Text = currentLineNumber.ToString();
        }

        public int LineNumber
        {
            get
            {
                var s = this.lineNumberTextBox.Text;
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
            this.lineNumberLabel = new System.Windows.Forms.Label();
            this.lineNumberTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lineNumberLabel
            // 
            this.lineNumberLabel.Location = new System.Drawing.Point(8, 8);
            this.lineNumberLabel.Name = "lineNumberLabel";
            this.lineNumberLabel.Size = new System.Drawing.Size(128, 16);
            this.lineNumberLabel.TabIndex = 0;
            this.lineNumberLabel.Text = "Line number (1 - {0}):";
            // 
            // lineNumberTextBox
            // 
            this.lineNumberTextBox.Location = new System.Drawing.Point(8, 24);
            this.lineNumberTextBox.Name = "lineNumberTextBox";
            this.lineNumberTextBox.Size = new System.Drawing.Size(208, 20);
            this.lineNumberTextBox.TabIndex = 1;
            this.lineNumberTextBox.Text = "";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(56, 56);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(136, 56);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            // 
            // GotoLineForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(224, 86);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.lineNumberTextBox);
            this.Controls.Add(this.lineNumberLabel);
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
                var s = this.lineNumberTextBox.Text;
                var lineNumber = int.Parse(s);

                if (lineNumber >= 1 && lineNumber <= this.maxLineLineNumber)
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch
            {
            }
        }

		private int maxLineLineNumber;
    }
}