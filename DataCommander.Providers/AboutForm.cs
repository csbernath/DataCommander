namespace DataCommander.Providers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Windows.Forms;
    using DataCommander.Foundation.Diagnostics;

    public partial class AboutForm : Form
    {
        private bool first = true;

        public AboutForm()
        {
            var assembly = Assembly.GetEntryAssembly();
            var path = assembly.Location;
            var lastWriteTime = File.GetLastWriteTime(path);
            var dotNetFrameworkVersion = AppDomainMonitor.DotNetFrameworkVersion;
            var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();

            var text =
                $@"
<style>
    a {{text-decoration:none}}
</style>
<div style=""font-family:verdana;font-size:9pt"">
<a href=""https://github.com/csbernath/DataCommander"">Data Commander</a>
<br/>
<br/>
Including <a href=""https://github.com/csbernath/DataCommander/blob/master/DataCommander.Foundation/README.md"">Foundation Class Library</a>
<br/>
<br/>
Build date: {lastWriteTime.ToString("yyyy-MM-dd")}
<br/>
<br/>
Copyright © 2002-2016 <a href=""mailto://csaba.bernath@gmail.com"">Csaba Bernáth</a>
<br/>
This program is freeware and released under the <a href=""https://www.gnu.org/licenses/gpl.txt"">GNU General Public Licence</a>.
<br/>
<br/>
Target Framework: {targetFrameworkAttribute.FrameworkDisplayName}
<br/>
<br/>
<a href=""applicationdatafile://"">Application Data file</a>
<br/>
<a href=""logfile://"">Log file</a>
<br/>
<br/>
<table style=""font-family:verdana;font-size:9pt"">
<tr><td>.NET CLR version:</td><td>{Environment.Version}</td></tr>
<tr><td>.NET Framework version:</td><td>{dotNetFrameworkVersion}</td></tr>
<tr><td>.NET Processor architecture:</td><td>{assembly.GetName().ProcessorArchitecture}</td></tr>
</table>
</br>
Credits:
</br>
<ul style=""list-style-type:none"">
    <li><a href=""https://www.visualstudio.com/vs/community/"">Visual Studio Community 2017</a></li>                  
    <li><a href=""http://epplus.codeplex.com"">EPPlus Excel generator</a></li>
    <li><a href=""https://system.data.sqlite.org"">SQLite provider</a></li>
    <li><a href=""https://www.nuget.org/packages/MySql.Data/"">MySQL provider</a></li>
    <li><a href=""http://npgsql.projects.pgfoundry.org/"">PostgreSQL provider</a></li>  
</ul>
</div>";

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
                var exists = false;

                if (e.Url.Scheme == "applicationdatafile")
                {
                    var applicationDataFileName = DataCommanderApplication.Instance.FileName;
                    exists = true;
                    url = applicationDataFileName;
                }
                else if (e.Url.Scheme == "logfile")
                {
                    var logFileName = LogFactory.Instance.FileName;
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