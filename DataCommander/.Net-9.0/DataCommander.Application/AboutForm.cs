using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DataCommander.Api;
using Foundation.Diagnostics;
using Foundation.Log;
using Foundation.Windows.Forms;

namespace DataCommander.Application;

public partial class AboutForm : Form
{
    private bool _first = true;

    public AboutForm(ColorTheme? colorTheme)
    {
        var assembly = Assembly.GetEntryAssembly()!;
        var path = assembly.Location;
        var lastWriteTime = File.GetLastWriteTime(path);
        var windowsCurrentVersion = WindowsCurrentVersionRepository.Get();
        var buildNumber = int.Parse(windowsCurrentVersion.CurrentBuild!);
        var windowsName =
            WindowsNameCalculator.GetWindowsNameFromBuildNumber(buildNumber, windowsCurrentVersion.EditionId, windowsCurrentVersion.DisplayVersion);
        var brightness = colorTheme?.BackColor.GetBrightness();

        var bodyStyle = brightness < 0.12f
            ? "body {background-color: #202020;color:#dcdcdc}"
            : null;

        var text =
            $@"
<style>
    {bodyStyle}
    a {{text-decoration:none}}
</style>
<div style=""font-family:verdana;font-size:9pt"">
Data Commander Version: {assembly.GetName().Version} ({lastWriteTime:yyyy-MM-dd})<br/>
<a href=""https://github.com/csbernath/DataCommander"">GitHub repository</a><br/>
Copyright © 2002-2024 <a href=""mailto://csaba.bernath@gmail.com"">Csaba Bernáth</a><br/><br/>
<a href=""https://en.wikipedia.org/wiki/Free_and_open-source_software"">Free and open-source software</a><br/>
This program is released under the <a href=""https://www.gnu.org/licenses/gpl.txt"">GNU General Public Licence</a>.<br/><br/>
Including <a href=""https://github.com/csbernath/DataCommander/blob/master/Foundation/.Net-9.0/README.md"">Foundation Class Library</a><br/><br/>
<a href=""applicationdatafile://"">Application Data file</a><br/>
<a href=""logfile://"">Log file</a><br/><br/>
<table style=""font-family:verdana;font-size:9pt"">
<tr><td>Windows Name:</td><td>{windowsName}</td></tr>
<tr><td>Windows CurrentBuild:</td><td>{windowsCurrentVersion.CurrentBuild}</td></tr>
<tr><td>.NET CLR version:</td><td>{Environment.Version}</td></tr>
<tr><td>.NET Runtime:</td><td>{DotNetVersionGetter.GetDotNetRuntimeVersion()}</td></tr>
<tr><td>.NET Desktop Runtime:</td><td>{DotNetDesktopRuntimeVersionGetter.GetDotNetDesktopRuntimeVersion()}</td></tr>
</table>
<br/>
Credits:
<ul style=""list-style-type:none"">
    <li><a href=""https://www.jetbrains.com/rider/"">JetBrains Rider</a></li>
    <li><a href=""https://www.visualstudio.com/vs/community/"">Visual Studio Community</a></li>
    <li><a href=""https://www.jetbrains.com/resharper/"">JetBrains R# ReSharper</a></li>
    <li><a href=""https://github.com/JanKallman/EPPlus"">EPPlus Excel generator</a></li>
    <li><a href=""https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli"">Microsoft SQLite provider</a></li>
    <li><a href=""https://www.nuget.org/packages/MySql.Data/"">MySQL provider</a></li>
    <li><a href=""https://github.com/npgsql/npgsql"">PostgreSQL provider</a></li>  
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

    private void webBrowser1_Navigating(object? sender, WebBrowserNavigatingEventArgs e)
    {
        if (_first)
        {
            _first = false;
        }
        else
        {
            string? url = null;
            var exists = false;

            switch (e.Url!.Scheme)
            {
                case "applicationdatafile":
                {
                    var applicationDataFileName = DataCommanderApplication.Instance.ApplicationDataFileName;
                    exists = true;
                    url = applicationDataFileName;
                    break;
                }
                case "logfile":
                {
                    var logFileName = LogFactory.Instance.FileName;
                    if (logFileName != null)
                    {
                        exists = true;
                        url = logFileName;
                    }

                    break;
                }
                default:
                {
                    exists = true;
                    url = e.Url.ToString();
                    break;
                }
            }

            if (exists)
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(processStartInfo);
            }

            e.Cancel = true;
        }
    }
}