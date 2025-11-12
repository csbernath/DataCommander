namespace DataCommander.Application
{
    sealed partial class CancelableOperationForm
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
            textBox = new System.Windows.Forms.TextBox();
            elapsedTimeTextBox = new System.Windows.Forms.TextBox();
            cancelButton = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // textBox
            // 
            textBox.Dock = System.Windows.Forms.DockStyle.Top;
            textBox.Location = new System.Drawing.Point(0, 0);
            textBox.Multiline = true;
            textBox.Name = "textBox";
            textBox.ReadOnly = true;
            textBox.Size = new System.Drawing.Size(586, 175);
            textBox.TabIndex = 0;
            // 
            // elapsedTimeTextBox
            // 
            elapsedTimeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            elapsedTimeTextBox.Location = new System.Drawing.Point(91, 191);
            elapsedTimeTextBox.Name = "elapsedTimeTextBox";
            elapsedTimeTextBox.ReadOnly = true;
            elapsedTimeTextBox.Size = new System.Drawing.Size(46, 20);
            elapsedTimeTextBox.TabIndex = 1;
            elapsedTimeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(245, 190);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(94, 29);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 190);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(86, 17);
            label1.TabIndex = 3;
            label1.Text = "Elapsed time:";
            // 
            // CancelableOperationForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new System.Drawing.Size(586, 231);
            ControlBox = false;
            Controls.Add(label1);
            Controls.Add(cancelButton);
            Controls.Add(elapsedTimeTextBox);
            Controls.Add(textBox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.TextBox elapsedTimeTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
    }
}