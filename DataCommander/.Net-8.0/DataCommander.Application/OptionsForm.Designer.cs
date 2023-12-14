
namespace DataCommander.Application
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.colorThemeLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.colorThemeComboBox = new System.Windows.Forms.ComboBox();
            this.changeFontButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // colorThemeLabel
            // 
            this.colorThemeLabel.AutoSize = true;
            this.colorThemeLabel.Location = new System.Drawing.Point(25, 27);
            this.colorThemeLabel.Name = "colorThemeLabel";
            this.colorThemeLabel.Size = new System.Drawing.Size(66, 13);
            this.colorThemeLabel.TabIndex = 1;
            this.colorThemeLabel.Text = "Color theme:";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(225, 93);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(306, 93);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // colorThemeComboBox
            // 
            this.colorThemeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorThemeComboBox.FormattingEnabled = true;
            this.colorThemeComboBox.Items.AddRange(new object[] {
            "Light",
            "Dark"});
            this.colorThemeComboBox.Location = new System.Drawing.Point(106, 27);
            this.colorThemeComboBox.Name = "colorThemeComboBox";
            this.colorThemeComboBox.Size = new System.Drawing.Size(121, 21);
            this.colorThemeComboBox.TabIndex = 4;
            // 
            // changeFontButton
            // 
            this.changeFontButton.Location = new System.Drawing.Point(106, 55);
            this.changeFontButton.Name = "changeFontButton";
            this.changeFontButton.Size = new System.Drawing.Size(88, 23);
            this.changeFontButton.TabIndex = 5;
            this.changeFontButton.Text = "Change Font";
            this.changeFontButton.UseVisualStyleBackColor = true;
            this.changeFontButton.Click += new System.EventHandler(this.changeFontButton_Click);
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(392, 128);
            this.Controls.Add(this.changeFontButton);
            this.Controls.Add(this.colorThemeComboBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.colorThemeLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Data Commander options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label colorThemeLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox colorThemeComboBox;
        private System.Windows.Forms.Button changeFontButton;
    }
}