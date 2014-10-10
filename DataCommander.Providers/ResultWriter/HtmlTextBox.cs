namespace DataCommander.Providers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using DataCommander.Foundation.Diagnostics;

    /// <summary>
    /// Summary description for HtmlTextBox.
    /// </summary>
    internal sealed class HtmlTextBox : System.Windows.Forms.UserControl
    {
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private WebBrowser webBrowser;
        private string fileName;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// 
        /// </summary>
        public HtmlTextBox()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call
        }

        /// <summary>
        /// 
        /// </summary>
        public string Xml
        {
            set
            {
                fileName = Path.GetTempFileName();
                fileName += ".xml";
                FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);

                using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    streamWriter.Write(value);
                    streamWriter.Close();
                }
                
                this.webBrowser.Navigate(fileName);
            }
        }

        public void Navigate(string path)
        {
            this.webBrowser.Navigate(path);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (File.Exists(this.fileName))
                {
                    try
                    {
                        File.Delete(this.fileName);
                    }
                    catch(Exception e)
                    {
                        log.Write(LogLevel.Error, e.ToString());
                    }
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HtmlTextBox));
            this.webBrowser = new WebBrowser();
            GarbageMonitor.Add( "webBrowser", this.webBrowser );
            // ((System.ComponentModel.ISupportInitialize)(this.webBrowser)).BeginInit();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.webBrowser.Enabled = true;
            //this.webBrowser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("webBrowser.OcxState")));
            this.webBrowser.Size = new System.Drawing.Size(464, 184);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.DocumentCompleted += this.webBrowser_DocumentCompleted;

            // 
            // HtmlTextBox
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.webBrowser});
            this.Name = "HtmlTextBox";
            this.Size = new System.Drawing.Size(464, 184);
            // ((System.ComponentModel.ISupportInitialize)(this.webBrowser)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (fileName != null)
            {
                File.Delete(fileName);
            }
        }
    }
}