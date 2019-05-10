using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Foundation.Diagnostics;
using Foundation.Log;

namespace DataCommander.Providers
{
    public partial class AboutForm : Form
    {
        private bool _first = true;

        public AboutForm()
        {
            var assembly = Assembly.GetEntryAssembly();
            var path = assembly.Location;
            var lastWriteTime = File.GetLastWriteTime(path);
            var dotNetFrameworkRelease = AppDomainMonitor.GetDotNetFrameworkRelease();
            DotNetFrameworkVersionStore.TryGet(dotNetFrameworkRelease, out var dotNetFrameworkVersion);
            var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
            var windowsVersionInfo = WindowsVersionInfo.Get();

            var text =
                $@"
<style>
    a {{text-decoration:none}}
</style>
<div style=""font-family:verdana;font-size:9pt"">
<a href=""https://github.com/csbernath/DataCommander"">Data Commander</a>
<br/>
<br/>
Including <a href=""https://github.com/csbernath/DataCommander/blob/master/Foundation/.NetStandard-2.0/README.md"">Foundation (.NET Standard 2.0) Class Library</a>
<br/>
<br/>
Version: {assembly.GetName().Version}
<br/>
<br/>
Build date: {lastWriteTime.ToString("yyyy-MM-dd")}
<br/>
<br/>
Copyright © 2002-2019 <a href=""mailto://csaba.bernath@gmail.com"">Csaba Bernáth</a>
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
<tr><td>Windows ProductName:</td><td>{windowsVersionInfo.ProductName}</td></tr>
<tr><td>Windows ReleaseId:</td><td>{windowsVersionInfo.ReleaseId}</td></tr>
<tr><td>Windows CurrentBuild:</td><td>{windowsVersionInfo.CurrentBuild}</td></tr>
<tr><td>.NET CLR version:</td><td>{Environment.Version}</td></tr>
<tr><td>.NET Framework release:</td><td>{dotNetFrameworkRelease}</td></tr>
<tr><td>.NET Framework version:</td><td>{dotNetFrameworkVersion}</td></tr>
<tr><td>.NET Processor architecture:</td><td>{assembly.GetName().ProcessorArchitecture}</td></tr>
</table>
</br>
Credits:
</br>
<ul style=""list-style-type:none"">
    <li><a href=""https://www.visualstudio.com/vs/community/"">Visual Studio Community 2019</a></li>
    <li><a href=""https://www.jetbrains.com/resharper/"">JetBrains R# ReSharper</a></li>
    <li><a href=""https://github.com/JanKallman/EPPlus"">EPPlus Excel generator</a></li>
    <li><a href=""https://system.data.sqlite.org"">SQLite provider</a></li>
    <li><a href=""https://www.nuget.org/packages/MySql.Data/"">MySQL provider</a></li>
    <li><a href=""http://npgsql.projects.pgfoundry.org/"">PostgreSQL provider</a></li>  
</ul>
</div>";

            InitializeComponent();

            webBrowser1.DocumentText = text;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (_first)
            {
                _first = false;
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