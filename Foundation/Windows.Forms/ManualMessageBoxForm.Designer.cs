namespace Foundation.Windows.Forms
{
    partial class ManualMessageBoxForm
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
            pictureBox = new System.Windows.Forms.PictureBox();
            label1 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            button3 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.Location = new System.Drawing.Point(12, 8);
            pictureBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new System.Drawing.Size(60, 60);
            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(70, 8);
            label1.MaximumSize = new System.Drawing.Size(279, 87);
            label1.MinimumSize = new System.Drawing.Size(86, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(273, 26);
            label1.TabIndex = 1;
            label1.Text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            label1.Click += label1_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(button1);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 91);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(346, 36);
            panel1.TabIndex = 2;
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(201, 7);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(64, 20);
            button3.TabIndex = 2;
            button3.Text = "&No";
            button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(270, 7);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(64, 20);
            button2.TabIndex = 1;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(131, 7);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(64, 20);
            button1.TabIndex = 0;
            button1.Text = "&Yes";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // ManualMessageBoxForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(346, 127);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(pictureBox);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ManualMessageBoxForm";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
    }
}