using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DataCommander.Providers.Connection
{
    public class PasswordForm : Form
    {
        private TextBox _tbPassword;
        private Button _btnOk;
        private Button _btnCancel;
        private Label _label1;
        private readonly Container _components = null;

        public PasswordForm()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                if (_components != null)
                    _components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._tbPassword = new System.Windows.Forms.TextBox();
            this._btnOk = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbPassword
            // 
            this._tbPassword.Location = new System.Drawing.Point(112, 8);
            this._tbPassword.Name = "_tbPassword";
            this._tbPassword.PasswordChar = '*';
            this._tbPassword.Size = new System.Drawing.Size(176, 21);
            this._tbPassword.TabIndex = 0;
            this._tbPassword.Text = String.Empty;
            // 
            // btnOK
            // 
            this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOk.Location = new System.Drawing.Point(64, 40);
            this._btnOk.Name = "_btnOk";
            this._btnOk.TabIndex = 1;
            this._btnOk.Text = "OK";
            // 
            // btnCancel
            // 
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(152, 40);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.TabIndex = 2;
            this._btnCancel.Text = "Cancel";
            // 
            // label1
            // 
            this._label1.Location = new System.Drawing.Point(8, 12);
            this._label1.Name = "_label1";
            this._label1.Size = new System.Drawing.Size(88, 16);
            this._label1.TabIndex = 3;
            this._label1.Text = "Enter password:";
            // 
            // PasswordForm
            // 
            this.AcceptButton = this._btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 74);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this._label1,
                                                                  this._btnCancel,
                                                                  this._btnOk,
                                                                  this._tbPassword});
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PasswordForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PasswordForm";
            this.ResumeLayout(false);

        }
        #endregion

        public string Password => _tbPassword.Text;
    }
}