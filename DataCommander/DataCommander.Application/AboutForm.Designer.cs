using System.ComponentModel;
using System.Windows.Forms;

namespace DataCommander.Application
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            webBrowser1 = new System.Windows.Forms.WebBrowser();
            SuspendLayout();
            // 
            // webBrowser1
            // 
            webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            webBrowser1.Location = new System.Drawing.Point(0, 0);
            webBrowser1.Margin = new System.Windows.Forms.Padding(4);
            webBrowser1.MinimumSize = new System.Drawing.Size(23, 26);
            webBrowser1.Name = "webBrowser1";
            webBrowser1.ScrollBarsEnabled = false;
            webBrowser1.Size = new System.Drawing.Size(584, 521);
            webBrowser1.TabIndex = 0;
            webBrowser1.Navigating += webBrowser1_Navigating;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(584, 521);
            Controls.Add(webBrowser1);
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "About Data Commander";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;

    }
}