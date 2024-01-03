﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DataCommander.Api;
using Foundation.Diagnostics;
using Foundation.Log;

namespace DataCommander.Application;

public partial class AboutForm : Form
{
    private bool _first = true;

    public AboutForm(ColorTheme colorTheme)
    {
        var assembly = Assembly.GetEntryAssembly();
        var path = assembly.Location;
        var lastWriteTime = File.GetLastWriteTime(path);
        var windowsVersionInfo = WindowsVersionInfo.Get();
        var windowsName = GetWindowsNameFromCurrentBuildAndEditionId(windowsVersionInfo.CurrentBuild, windowsVersionInfo.EditionId);

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
Build date: {lastWriteTime.ToString("yyyy-MM-dd")}
<br/><br/>
Version: {assembly.GetName().Version}
<br/><br/>
Copyright © 2002-2024 <a href=""mailto://csaba.bernath@gmail.com"">Csaba Bernáth</a>
<br/>
This program is freeware and released under the <a href=""https://www.gnu.org/licenses/gpl.txt"">GNU General Public Licence</a>.
<br/><br/>
<a href=""https://github.com/csbernath/DataCommander"">GitHub repository</a>
<br/><br/>
Including <a href=""https://github.com/csbernath/DataCommander/blob/master/Foundation/.Net-8.0/README.md"">Foundation (.NET 8.0) Class Library</a>
<br/><br/>
<a href=""applicationdatafile://"">Application Data file</a>
<br/>
<a href=""logfile://"">Log file</a>
<br/><br/>
<table style=""font-family:verdana;font-size:9pt"">
<tr><td>Windows Name:</td><td>{windowsName}</td></tr>
<tr><td>Windows CurrentBuild:</td><td>{windowsVersionInfo.CurrentBuild}</td></tr>
<tr><td>.NET CLR version:</td><td>{Environment.Version}</td></tr>
</table>
<br/>
Credits:
<ul style=""list-style-type:none"">
    <li><a href=""https://www.jetbrains.com/rider/"">JetBrains Rider</a></li>
    <li><a href=""https://www.visualstudio.com/vs/community/"">Visual Studio Community</a></li>
    <li><a href=""https://www.jetbrains.com/resharper/"">JetBrains R# ReSharper</a></li>
    <li><a href=""https://github.com/JanKallman/EPPlus"">EPPlus Excel generator</a></li>
    <li><a href=""https://system.data.sqlite.org"">SQLite provider</a></li>
    <li><a href=""https://www.nuget.org/packages/MySql.Data/"">MySQL provider</a></li>
    <li><a href=""https://github.com/npgsql/npgsql"">PostgreSQL provider</a></li>  
</ul>
</div>";

        InitializeComponent();

        webBrowser1.DocumentText = text;
    }

    private static string GetWindowsNameFromCurrentBuildAndEditionId(string currentBuild, string editionId)
    {
        string windowsName = null;
        switch (currentBuild)
        {
            case "19044":
                windowsName = $"Windows 10 {editionId} version 21H2";
                break;
            case "19045":
                windowsName = $"Windows 10 {editionId} version 22H2";
                break;
            case "22000":
                windowsName = $"Windows 11 {editionId} version 21H2";
                break;
            case "22621":
                windowsName = $"Windows 11 {editionId} version 22H2";
                break;
            case "22631":
                windowsName = $"Windows 11 {editionId} version 23H2";
                break;
        }

        return windowsName;
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