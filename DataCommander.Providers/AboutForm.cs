namespace DataCommander.Providers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Web;
    using System.Windows.Forms;
    using DataCommander.Foundation.Diagnostics;
    using System.Linq;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Linq;

    public partial class AboutForm : Form
    {
        private bool first = true;

        public AboutForm()
        {
            var assembly = Assembly.GetEntryAssembly();
            string path = assembly.Location;
            DateTime lastWriteTime = File.GetLastWriteTime(path);
            string dotNetFrameworkVersion = AppDomainMonitor.DotNetFrameworkVersion;

            const string searchPattern = "DataCommander*.log";
            var directoryInfo = new DirectoryInfo(Path.GetTempPath());
            FileInfo[] fileInfos = directoryInfo.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
            var fileInfo = fileInfos.OrderByDescending(i => i.CreationTime).FirstOrDefault();
            string logFileName = null;
            if (fileInfo != null)
            {
                logFileName = fileInfo.FullName;
            }

            string text = string.Format(@"<div style=""font-family:verdana;font-size:9pt"">
<a href=""https://github.com/csbernath/DataCommander"">Data Commander</a>
<br/>
Copyright © 2002-2015 <a href=""mailto://csaba.bernath@gmail.com"">Csaba Bernáth</a>
<br/>
This program is freeware and released under the <a href=""https://www.gnu.org/licenses/gpl.txt"">GNU General Public Licence</a>.
<br/>
<br/>
Build date: {3}
<br/>
<br/>
<a href=""localfile://{4}"">Application Data file</a>
<br/>
<a href=""localfile://{6}"">Log file</a>
<br/>
<br/>
<table style=""font-family:verdana;font-size:9pt"">
<tr><td>.NET CLR version:</td><td>{0}</td></tr>
<tr><td>.NET Framework version:</td><td>{1}</td></tr>
<tr><td>.NET Processor architecture:</td><td>{2}</td></tr>
<tr><td>Allocated physical memory:</td><td>{5} MB</td></tr>
</table>
</br>
Credits:
</br>
<ul style=""list-style-type:none"">
    <li><a href=""http://epplus.codeplex.com"">EPPlus-Create advanced Excel spreadsheets on the server</a></li>
    <li><a href=""https://system.data.sqlite.org"">System.Data.SQLite</a></li>
</ul>
</div>",
                Environment.Version,
                dotNetFrameworkVersion,
                assembly.GetName().ProcessorArchitecture,
                lastWriteTime.ToLongDateString(),
                HttpUtility.HtmlEncode(Application.Instance.FileName),
                ((double)Environment.WorkingSet/(1024*1024)).ToString("N0"),
                logFileName);
            
            InitializeComponent();

            this.webBrowser1.DocumentText = text;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                string url;

                if (e.Url.Scheme == "localfile")
                {
                    var uriBuilder = new UriBuilder(e.Url);
                    uriBuilder.Scheme = "file";
                    //url = uriBuilder.Uri.ToString();
                    url = HttpUtility.UrlDecode(uriBuilder.Path);
                }
                else
                {
                    url = e.Url.ToString();
                }

                Process.Start(url);
                e.Cancel = true;
            }
        }
    }
}