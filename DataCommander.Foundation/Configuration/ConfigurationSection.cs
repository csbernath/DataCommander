namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Xml;
    using DataCommander.Foundation.Diagnostics;
    using FileSystemWatcher = DataCommander.Foundation.IO.FileSystemWatcher;

    /// <summary>
    /// Loads and parses the <c><DataCommander.Foundation.Configuration /></c> xml element from the config file.
    /// </summary>
    /// <remarks>
    /// <code escaped="true">
    /// <SectionName1>
    ///   <NodeName1>
    ///            <NodeName11>
    ///                <attribute name="" type="" value="" />
    ///            </NodeName11>
    ///   </NodeName1>
    ///   ...
    ///   <NodeNameN>
    ///   </NodeNameN>
    /// </SectionName1>
    /// </code>
    /// 
    /// Available attribute types: 
    /// 
    /// 
    /// <list type="table">
    ///        <listheader>
    ///            <term>Type</term>
    ///            <description>Description</description>
    ///        </listheader>
    ///        <item>
    ///            <term>
    ///                <see cref="bool">bool</see>,
    ///                <see cref="int">int</see>,
    ///                <see cref="string">string</see>
    ///                etc.
    ///            </term>
    ///            <description>See <see href="ms-help://MS.MSDNQTR.2003APR.1033/csref/html/vclrfbuiltintypes.htm">Built-in Types Table</see> in C# Programmer's Reference.</description>
    ///        </item>
    ///        <item>
    ///            <term>arrays like
    ///                <see cref="bool">bool</see>[],
    ///                <see cref="int">int</see>[],
    ///                <see cref="string">string</see>[]
    ///                etc.
    ///            </term>
    ///            <description></description>
    ///        </item>
    ///        <item>
    ///            <term><see cref="byte">byte</see>[]</term>
    ///            <description>base64 encoded byte array</description>
    ///        </item>
    ///        <item>
    ///            <term>datetime</term>
    ///            <description><see cref="DateTime"/></description>
    ///        </item>
    ///        <item>
    ///            <term><see cref="TimeSpan">System.TimeSpan</see></term>
    ///            <description></description>
    ///        </item>
    ///        <item>
    ///            <term>enumeration types</term>
    ///            <description></description>
    ///        </item>
    ///        <item>
    ///            <term>xmlnode</term>
    ///            <description><see cref="XmlNode"/></description>
    ///        </item>
    ///        <item>
    ///            <term></term>
    ///            <description></description>
    ///        </item>
    ///        <item>
    ///            <term></term>
    ///            <description></description>
    ///        </item>
    ///        <item>
    ///            <term></term>
    ///            <description></description>
    ///        </item>
    /// </list>
    /// </remarks>
    public sealed class ConfigurationSection
    {
        private static readonly ILog log = InternalLogFactory.Instance.GetTypeLog(typeof (ConfigurationSection));
        private int changed;

        /// <summary>
        /// Reads configuration settings from the specified <paramref name="configFileName"/>.
        /// </summary>
        /// <param name="configFileName"></param>
        /// <param name="sectionName"></param>
        public ConfigurationSection(string configFileName, string sectionName)
        {
            this.ConfigFileName = configFileName;
            this.SectionName = sectionName;
            this.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFileName"></param>
        public ConfigurationSection(string configFileName)
        {
            this.ConfigFileName = configFileName;
            this.SectionName = DefaultSectionName;
            this.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public static string DefaultSectionName
        {
            get
            {
                var sectionName = typeof(ConfigurationSection).Namespace;
                return sectionName;
            }
        }

        /// <summary>
        /// RootNode can inform the caller about config file changes here.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationNode RootNode { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationNode CurrentNamespace
        {
            get
            {
                var trace = new StackTrace(1);
                var nodeName = ConfigurationNodeName.FromNamespace(trace, 0);
                var node = this.SelectNode(nodeName, true);
                return node;
            }
        }

        /// <summary>
        /// Gets the node correspoding to the caller's type.
        /// </summary>
        public ConfigurationNode CurrentType
        {
            get
            {
                var trace = new StackTrace(1);
                var nodeName = ConfigurationNodeName.FromType(trace, 0);
                var node = this.SelectNode(nodeName, true);
                return node;
            }
        }

        /// <summary>
        /// Gets the node corresponding to the caller's method.
        /// </summary>
        public ConfigurationNode CurrentMethod
        {
            get
            {
                var trace = new StackTrace(1);
                var nodeName = ConfigurationNodeName.FromMethod(trace, 0);
                var node = this.SelectNode(nodeName, true);
                return node;
            }
        }

        /// <summary>
        /// Gets the name of file which the config is loaded from.
        /// </summary>
        public string ConfigFileName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string SectionName { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFileSystemWatcherEnabled { get; private set; }

        private void Check(string nodeName, ConfigurationNode node)
        {
            if (node == null)
            {
                if (!File.Exists(this.ConfigFileName))
                {
                    throw new FileNotFoundException("Configuration file not found.", this.ConfigFileName);
                }
                else
                {
                    throw new ArgumentException($"Configuration node not found.\r\nNodeName: {nodeName}\r\nConfigFileName: {this.ConfigFileName}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public ConfigurationNode SelectNode(string nodeName, bool throwOnError)
        {
            if (this.changed != 0)
            {
                lock (this)
                {
                    ConfigurationNode rootNode;
                    StringCollection fileNames;
                    this.Load(out rootNode, out fileNames);
                    this.RootNode = rootNode;
                }

                Interlocked.Exchange(ref this.changed, 0);
            }

            ConfigurationNode node;

            if (this.RootNode != null)
            {
                node = this.RootNode.SelectNode(nodeName);
            }
            else
            {
                node = null;
            }

            if (throwOnError)
            {
                this.Check(nodeName, node);
            }

            return node;
        }

        private void Initialize()
        {
            ConfigurationNode rootNode = null;
            StringCollection fileNames = null;

            try
            {
                this.Load(out rootNode, out fileNames);
                this.RootNode = rootNode;
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToString());
            }

            if (fileNames != null)
            {
                try
                {
                    foreach (var fileName in fileNames)
                    {
                        var watcher = new FileSystemWatcher(fileName);
                        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
                        watcher.Changed += this.OnChanged;
                        watcher.EnableRaisingEvents = true;
                    }

                    this.IsFileSystemWatcherEnabled = fileNames.Count > 0;
                }
                catch (Exception e)
                {
                    log.Write(LogLevel.Error, e.ToString());
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            log.Trace("Settings.OnChanged. FileName: " + e.FullPath);
            Interlocked.Increment(ref this.changed);

            if (this.Changed != null)
            {
                this.Changed(this, e);
            }
        }

        private void Load(out ConfigurationNode rootNode, out StringCollection fileNames)
        {
            var reader = new ConfigurationReader();
            fileNames = new StringCollection();
            rootNode = reader.Read(this.ConfigFileName, this.SectionName, fileNames);
        }
    }
}