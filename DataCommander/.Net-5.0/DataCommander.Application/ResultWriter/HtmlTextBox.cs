using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Foundation.Diagnostics;
using Foundation.Log;

namespace DataCommander.Application.ResultWriter;

/// <summary>
/// Summary description for HtmlTextBox.
/// </summary>
internal sealed class HtmlTextBox : UserControl
{
    #region Private Fields

    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private WebBrowser _webBrowser;
    private string _fileName;

    #endregion

    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private readonly Container _components = null;

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
            _fileName = Path.GetTempFileName();
            _fileName += ".xml";

            using (var fileStream = new FileStream(_fileName, FileMode.OpenOrCreate))
            {
                using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    streamWriter.Write(value);
                    streamWriter.Close();
                }
            }

            _webBrowser.Navigate(_fileName);
        }
    }

    public void Navigate(string path)
    {
        _webBrowser.Navigate(path);
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (File.Exists(_fileName))
            {
                try
                {
                    File.Delete(_fileName);
                }
                catch (Exception e)
                {
                    Log.Write(LogLevel.Error, e.ToString());
                }
            }

            if (_components != null)
            {
                _components.Dispose();
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
        var resources = new System.Resources.ResourceManager(typeof(HtmlTextBox));
        this._webBrowser = new WebBrowser();
        GarbageMonitor.Default.Add("webBrowser", this._webBrowser);
        // ((System.ComponentModel.ISupportInitialize)(this.webBrowser)).BeginInit();
        this.SuspendLayout();
        // 
        // webBrowser
        // 
        this._webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
        //this.webBrowser.Enabled = true;
        //this.webBrowser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("webBrowser.OcxState")));
        this._webBrowser.Size = new System.Drawing.Size(464, 184);
        this._webBrowser.TabIndex = 0;
        this._webBrowser.DocumentCompleted += this.webBrowser_DocumentCompleted;

        // 
        // HtmlTextBox
        // 
        this.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            this._webBrowser
        });
        this.Name = "HtmlTextBox";
        this.Size = new System.Drawing.Size(464, 184);
        // ((System.ComponentModel.ISupportInitialize)(this.webBrowser)).EndInit();
        this.ResumeLayout(false);

    }

    #endregion

    private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
        if (_fileName != null)
        {
            File.Delete(_fileName);
        }
    }
}