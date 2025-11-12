
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            colorThemeLabel = new System.Windows.Forms.Label();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            colorThemeComboBox = new System.Windows.Forms.ComboBox();
            changeFontButton = new System.Windows.Forms.Button();
            initializeApplicationConfigurationCheckBox = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // colorThemeLabel
            // 
            colorThemeLabel.AutoSize = true;
            colorThemeLabel.Location = new System.Drawing.Point(13, 12);
            colorThemeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            colorThemeLabel.Name = "colorThemeLabel";
            colorThemeLabel.Size = new System.Drawing.Size(76, 15);
            colorThemeLabel.TabIndex = 1;
            colorThemeLabel.Text = "Color theme:";
            // 
            // okButton
            // 
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Location = new System.Drawing.Point(262, 107);
            okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(88, 27);
            okButton.TabIndex = 2;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(357, 107);
            cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(88, 27);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // colorThemeComboBox
            // 
            colorThemeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            colorThemeComboBox.FormattingEnabled = true;
            colorThemeComboBox.Items.AddRange(new object[] { "Light", "Dark" });
            colorThemeComboBox.Location = new System.Drawing.Point(210, 9);
            colorThemeComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            colorThemeComboBox.Name = "colorThemeComboBox";
            colorThemeComboBox.Size = new System.Drawing.Size(140, 23);
            colorThemeComboBox.TabIndex = 4;
            // 
            // changeFontButton
            // 
            changeFontButton.Location = new System.Drawing.Point(13, 64);
            changeFontButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            changeFontButton.Name = "changeFontButton";
            changeFontButton.Size = new System.Drawing.Size(103, 27);
            changeFontButton.TabIndex = 5;
            changeFontButton.Text = "Change Font";
            changeFontButton.UseVisualStyleBackColor = true;
            changeFontButton.Click += changeFontButton_Click;
            // 
            // initializeApplicationConfigurationCheckBox
            // 
            initializeApplicationConfigurationCheckBox.AutoSize = true;
            initializeApplicationConfigurationCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            initializeApplicationConfigurationCheckBox.Location = new System.Drawing.Point(13, 39);
            initializeApplicationConfigurationCheckBox.Name = "initializeApplicationConfigurationCheckBox";
            initializeApplicationConfigurationCheckBox.Size = new System.Drawing.Size(210, 19);
            initializeApplicationConfigurationCheckBox.TabIndex = 6;
            initializeApplicationConfigurationCheckBox.Text = "Initialize Application Configuration";
            initializeApplicationConfigurationCheckBox.UseVisualStyleBackColor = true;
            initializeApplicationConfigurationCheckBox.CheckedChanged += initializeApplicationConfigurationCheckBox_CheckedChanged;
            // 
            // OptionsForm
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new System.Drawing.Size(457, 148);
            Controls.Add(initializeApplicationConfigurationCheckBox);
            Controls.Add(changeFontButton);
            Controls.Add(colorThemeComboBox);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(colorThemeLabel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OptionsForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Data Commander options";
            ResumeLayout(false);
            PerformLayout();

        }

        private System.Windows.Forms.Label colorThemeLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox colorThemeComboBox;
        private System.Windows.Forms.Button changeFontButton;
        private System.Windows.Forms.CheckBox initializeApplicationConfigurationCheckBox;
    }
}