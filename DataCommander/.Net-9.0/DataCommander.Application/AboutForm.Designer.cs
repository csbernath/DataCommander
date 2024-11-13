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
            webBrowser1 = new WebBrowser();
            SuspendLayout();
            // 
            // webBrowser1
            // 
            webBrowser1.Dock = DockStyle.Fill;
            webBrowser1.Location = new System.Drawing.Point(0, 0);
            webBrowser1.Margin = new Padding(4);
            webBrowser1.MinimumSize = new System.Drawing.Size(23, 26);
            webBrowser1.Name = "webBrowser1";
            webBrowser1.ScrollBarsEnabled = false;
            webBrowser1.Size = new System.Drawing.Size(584, 509);
            webBrowser1.TabIndex = 0;
            webBrowser1.Navigating += webBrowser1_Navigating;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(584, 509);
            Controls.Add(webBrowser1);
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About Data Commander";
            ResumeLayout(false);
        }

        #endregion

        private WebBrowser webBrowser1;

    }
}