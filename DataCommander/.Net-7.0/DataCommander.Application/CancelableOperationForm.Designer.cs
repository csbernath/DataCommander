namespace DataCommander.Application
{
    partial class CancelableOperationForm
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
            SuspendLayout();
            // 
            // textBox
            // 
            textBox.Dock = System.Windows.Forms.DockStyle.Top;
            textBox.Location = new System.Drawing.Point(0, 0);
            textBox.Multiline = true;
            textBox.Name = "textBox";
            textBox.ReadOnly = true;
            textBox.Size = new System.Drawing.Size(586, 94);
            textBox.TabIndex = 0;
            // 
            // elapsedTimeTextBox
            // 
            elapsedTimeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            elapsedTimeTextBox.Location = new System.Drawing.Point(12, 194);
            elapsedTimeTextBox.Name = "elapsedTimeTextBox";
            elapsedTimeTextBox.ReadOnly = true;
            elapsedTimeTextBox.Size = new System.Drawing.Size(87, 20);
            elapsedTimeTextBox.TabIndex = 1;
            // 
            // cancelButton
            // 
            cancelButton.Location = new System.Drawing.Point(226, 190);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(94, 29);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // CancelableOperationForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(586, 231);
            ControlBox = false;
            Controls.Add(cancelButton);
            Controls.Add(elapsedTimeTextBox);
            Controls.Add(textBox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CancelableOperationForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.TextBox elapsedTimeTextBox;
        private System.Windows.Forms.Button cancelButton;
    }
}