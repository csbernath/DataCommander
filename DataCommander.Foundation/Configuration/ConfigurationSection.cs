namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Xml;
    using DataCommander.Foundation.Diagnostics;

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
    ///                <see cref="Boolean">Boolean</see>,
    ///                <see cref="Int32">Int32</see>,
    ///                <see cref="String">String</see>
    ///                etc.
    ///            </term>
    ///            <description>See <see href="ms-help://MS.MSDNQTR.2003APR.1033/csref/html/vclrfbuiltintypes.htm">Built-in Types Table</see> in C# Programmer's Reference.</description>
    ///        </item>
    ///        <item>
    ///            <term>arrays like
    ///                <see cref="Boolean">Boolean</see>[],
    ///                <see cref="Int32">Int32</see>[],
    ///                <see cref="String">String</see>[]
    ///                etc.
    ///            </term>
    ///            <description></description>
    ///        </item>
    ///        <item>
    ///            <term><see cref="Byte">Byte</see>[]</term>
    ///            <description>base64 encoded Byte array</description>
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
        private static ILog log = InternalLogFactory.Instance.GetCurrentTypeLog();
        private String configFileName;
        private String sectionName;
        private ConfigurationNode rootNode;
        private Int32 changed;
        private Boolean isFileSystemWatcherEnabled;

        /// <summary>
        /// Reads configuration settings from the specified <paramref name="configFileName"/>.
        /// </summary>
        /// <param name="configFileName"></param>
        /// <param name="sectionName"></param>
        public ConfigurationSection( String configFileName, String sectionName )
        {
            this.configFileName = configFileName;
            this.sectionName = sectionName;
            this.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFileName"></param>
        public ConfigurationSection( String configFileName )
        {
            this.configFileName = configFileName;
            this.sectionName = DefaultSectionName;
            this.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public static String DefaultSectionName
        {
            get
            {
                String sectionName = typeof( ConfigurationSection ).Namespace;
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
        public ConfigurationNode RootNode
        {
            get
            {
                return this.rootNode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationNode CurrentNamespace
        {
            get
            {
                var trace = new StackTrace( 1 );
                String nodeName = ConfigurationNodeName.FromNamespace( trace, 0 );
                ConfigurationNode node = this.SelectNode( nodeName, true );
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
                var trace = new StackTrace( 1 );
                String nodeName = ConfigurationNodeName.FromType( trace, 0 );
                ConfigurationNode node = this.SelectNode( nodeName, true );
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
                var trace = new StackTrace( 1 );
                String nodeName = ConfigurationNodeName.FromMethod( trace, 0 );
                ConfigurationNode node = this.SelectNode( nodeName, true );
                return node;
            }
        }

        /// <summary>
        /// Gets the name of file which the config is loaded from.
        /// </summary>
        public String ConfigFileName
        {
            get
            {
                return this.configFileName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String SectionName
        {
            get
            {
                return this.sectionName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsFileSystemWatcherEnabled
        {
            get
            {
                return this.isFileSystemWatcherEnabled;
            }
        }

        private void Check( String nodeName, ConfigurationNode node )
        {
            if (node == null)
            {
                if (!File.Exists( this.configFileName ))
                {
                    throw new FileNotFoundException( "Configuration file not found.", this.configFileName );
                }
                else
                {
                    throw new ArgumentException( String.Format(
                        "Configuration node not found.\r\nNodeName: {0}\r\nConfigFileName: {1}", nodeName, this.configFileName ) );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public ConfigurationNode SelectNode( String nodeName, Boolean throwOnError )
        {
            if (this.changed != 0)
            {
                lock (this)
                {
                    ConfigurationNode rootNode;
                    StringCollection fileNames;
                    this.Load( out rootNode, out fileNames );
                    this.rootNode = rootNode;
                }

                Interlocked.Exchange( ref this.changed, 0 );
            }

            ConfigurationNode node;

            if (this.rootNode != null)
            {
                node = this.rootNode.SelectNode( nodeName );
            }
            else
            {
                node = null;
            }

            if (throwOnError)
            {
                this.Check( nodeName, node );
            }

            return node;
        }

        private void Initialize()
        {
            ConfigurationNode rootNode = null;
            StringCollection fileNames = null;

            try
            {
                this.Load( out rootNode, out fileNames );
                this.rootNode = rootNode;
            }
            catch (Exception e)
            {
                log.Write( LogLevel.Error, e.ToString() );
            }

            if (fileNames != null)
            {
                try
                {
                    foreach (String fileName in fileNames)
                    {
                        var watcher = new IO.FileSystemWatcher( fileName );
                        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
                        watcher.Changed += this.OnChanged;
                        watcher.EnableRaisingEvents = true;
                    }

                    this.isFileSystemWatcherEnabled = fileNames.Count > 0;
                }
                catch (Exception e)
                {
                    log.Write( LogLevel.Error, e.ToString() );
                }
            }
        }

        private void OnChanged( Object sender, FileSystemEventArgs e )
        {
            log.Trace("Settings.OnChanged. FileName: " + e.FullPath );
            Interlocked.Increment( ref this.changed );

            if (this.Changed != null)
            {
                this.Changed( this, e );
            }
        }

        private void Load( out ConfigurationNode rootNode, out StringCollection fileNames )
        {
            ConfigurationReader reader = new ConfigurationReader();
            fileNames = new StringCollection();
            rootNode = reader.Read( this.configFileName, this.sectionName, fileNames );
        }
    }
}