namespace DataCommander.Providers.Query
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for CompletionForm.
    /// </summary>
    internal sealed class CompletionForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>kkjj
        private readonly Container components = null;

        private readonly QueryForm queryForm;
        private EventHandler<ItemSelectedEventArgs> itemSelectedEvent;

        public CompletionForm(QueryForm queryForm)
        {
            //
            // Required for Windows Form Designer support
            //
            this.InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.queryForm = queryForm;
        }

        public event EventHandler<ItemSelectedEventArgs> ItemSelected
        {
            add => this.itemSelectedEvent += value;

            remove => this.itemSelectedEvent -= value;
        }

        public void Initialize(QueryTextBox textBox, GetCompletionResponse response)
        {
            var listBox = new MemberListBox(this, textBox);
            listBox.Initialize(response);
            listBox.Dock = DockStyle.Fill;

            this.Controls.Add(listBox);

            var charIndex = textBox.RichTextBox.SelectionStart;
            var pos = textBox.RichTextBox.GetPositionFromCharIndex(charIndex);
            var location = textBox.RichTextBox.PointToScreen(pos);
            location.Y += 20;
            this.Location = location;
        }

        public void SelectItem(int startIndex, int length, IObjectName objectName)
        {
            if (this.itemSelectedEvent != null)
            {
                this.itemSelectedEvent(this, new ItemSelectedEventArgs(startIndex, length, objectName));
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
            this.SuspendLayout();
            // 
            // CompletionForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(400, 140);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CompletionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResumeLayout(false);

        }

        #endregion
    }
}