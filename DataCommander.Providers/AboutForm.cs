namespace DataCommander.Providers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Web;
    using System.Windows.Forms;
    using DataCommander.Foundation.Diagnostics;

    public partial class AboutForm : Form
    {
        private bool first = true;

        public AboutForm()
        {
            var assembly = Assembly.GetEntryAssembly();
            string path = assembly.Location;
            DateTime lastWriteTime = File.GetLastWriteTime(path);
            string dotNetFrameworkVersion = AppDomainMonitor.DotNetFrameworkVersion;
            var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();

            string text = string.Format(@"
<style>
    a {{text-decoration:none}}
</style>
<div style=""font-family:verdana;font-size:9pt"">
<a href=""https://github.com/csbernath/DataCommander"">Data Commander on GitHub</a>
<br/>
<br/>
Build date: {0}
<br/>
<br/>
Copyright © 2002-2015 <a href=""mailto://csaba.bernath@gmail.com"">Csaba Bernáth</a>
<br/>
This program is freeware and released under the <a href=""https://www.gnu.org/licenses/gpl.txt"">GNU General Public Licence</a>.
<br/>
<br/>
Target Framework: {1}
<br/>
<br/>
<a href=""localfile://{2}"">Application Data file</a>
<br/>
<a href=""logfile://"">Log file</a>
<br/>
<br/>
<table style=""font-family:verdana;font-size:9pt"">
<tr><td>.NET CLR version:</td><td>{3}</td></tr>
<tr><td>.NET Framework version:</td><td>{4}</td></tr>
<tr><td>.NET Processor architecture:</td><td>{5}</td></tr>
</table>
</br>
Credits:
</br>
<ul style=""list-style-type:none"">
    <li><a href=""https://www.visualstudio.com/products/visual-studio-community-vs"">Visual Studio 2015 Community Edition</a></li>                  
    <li><a href=""https://www.jetbrains.com/resharper/"">JetBrains R# ReSharper Ultimate Free Open Source License</a></li>
    <li><a href=""http://epplus.codeplex.com"">EPPlus-Create advanced Excel spreadsheets on the server</a></li>
    <li><a href=""https://system.data.sqlite.org"">System.Data.SQLite</a></li>
    <li><a href=""https://www.nuget.org/packages/MySql.Data/"">ADO.Net driver for MySQL</a></li>
    <li><a href=""http://npgsql.projects.pgfoundry.org/"">Npgsql - .Net Data Provider for Postgresql</a></li>
</ul>
</div>",
                lastWriteTime.ToString("yyyy-MM-dd"),
                targetFrameworkAttribute.FrameworkDisplayName,
                HttpUtility.HtmlEncode(DataCommanderApplication.Instance.FileName),
                Environment.Version,
                dotNetFrameworkVersion,
                assembly.GetName().ProcessorArchitecture);

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
                string url = null;
                bool exists = false;

                if (e.Url.Scheme == "localfile")
                {
                    var uriBuilder = new UriBuilder(e.Url);
                    uriBuilder.Scheme = "file";
                    url = HttpUtility.UrlDecode(uriBuilder.Path);
                    exists = File.Exists(url);
                }
                else if (e.Url.Scheme == "logfile")
                {
                    string logFileName = LogFactory.Instance.FileName;
                    if (logFileName != null)
                    {
                        exists = true;
                        url = logFileName;
                    }
                }
                else
                {
                    exists = true;
                    url = e.Url.ToString();
                }

                if (exists)
                {
                    Process.Start(url);
                }

                e.Cancel = true;
            }
        }
    }
}