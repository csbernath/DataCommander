namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// This class is a singleton which holds a <see cref="ConfigurationSection"/> instance.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// The config file name.
        /// </summary>
        private static string configFileName;

        private static string sectionName;

        /// <summary>
        /// The ConfigurationSection instance.
        /// </summary>
        private static ConfigurationSection section;

        /// <summary>
        /// 
        /// </summary>
        public static event EventHandler Changed
        {
            add
            {
                Section.Changed += value;
            }

            remove
            {
                Section.Changed -= value;
            }
        }

        /// <summary>
        /// Uses <see cref="AppSettings"/> to retrieve "ConfigFileName" from the app.config file.
        /// </summary>
        public static string ConfigFileName
        {
            get
            {
                if (configFileName == null)
                {
                    AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                    bool contains;

                    try
                    {
                        contains = AppSettings.CurrentType.TryGetString("ConfigFileName", out configFileName);
                    }
                    catch
                    {
                        contains = false;
                    }

                    if (contains && !string.IsNullOrEmpty(configFileName))
                    {
                        configFileName = Path.Combine(setup.ApplicationBase, configFileName);
                    }
                    else
                    {
                        configFileName = setup.ConfigurationFile;
                    }
                }

                return configFileName;
            }

            set
            {
                configFileName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static ConfigurationNode RootNode
        {
            get
            {
                return Section.RootNode;
            }
        }

        /// <summary>
        /// Gets the <see cref="ConfigurationSection" /> instance.
        /// Initializes the instance at first call.
        /// </summary>
        public static ConfigurationSection Section
        {
            get
            {
                if (section == null)
                {
                    string configFileName = ConfigFileName;
                    string sectionName = SectionName;
                    section = new ConfigurationSection(configFileName, sectionName);
                }

                return section;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string SectionName
        {
            get
            {
                if (sectionName == null)
                {
                    sectionName = ConfigurationSection.DefaultSectionName;
                }

                return sectionName;
            }

            set
            {
                sectionName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static ConfigurationNode CurrentMethod
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                var trace = new StackTrace(1);
                string nodeName = ConfigurationNodeName.FromMethod(trace, 0);
                ConfigurationNode node = Section.SelectNode(nodeName, true);
                return node;
            }
        }

        /// <summary>
        /// Gets the config node of the calling method's type.
        /// </summary>
        public static ConfigurationNode CurrentType
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                var trace = new StackTrace(1);
                string nodeName = ConfigurationNodeName.FromType(trace, 0);
                ConfigurationNode node = Section.SelectNode(nodeName, true);
                return node;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static ConfigurationNode CurrentNamespace
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                var trace = new StackTrace(1);
                string nodeName = ConfigurationNodeName.FromNamespace(trace, 0);
                ConfigurationNode node = Section.SelectNode(nodeName, true);
                return node;
            }
        }

        /// <summary>
        /// Computes the config file name from the given assembly's CodeBase.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetAssemblyConfigFileName(Assembly assembly)
        {
            Contract.Requires<ArgumentNullException>(assembly != null);

            string codeBase = assembly.CodeBase;
            Uri uri = new Uri(codeBase);
            string fileName = uri.LocalPath;
            FileInfo fileInfo = new FileInfo(fileName);
            fileName = fileInfo.FullName;
            string configFilename = fileName + ".config";
            return configFilename;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public static ConfigurationNode SelectNode(string nodeName, bool throwOnError)
        {
            return Section.SelectNode(nodeName, throwOnError);
        }

        internal static ConfigurationNode SelectNode(Type type, bool throwOnError)
        {
            string nodeName = ConfigurationNodeName.FromType(type);
            ConfigurationNode node = Section.SelectNode(nodeName, throwOnError);
            return node;
        }

        /// <summary>
        /// Finds the config node of the calling method's type.
        /// </summary>
        /// <returns>null, if not found and no exception is thrown.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ConfigurationNode SelectCurrentType()
        {
            var trace = new StackTrace(1);
            string nodeName = ConfigurationNodeName.FromType(trace, 0);
            //log.Trace( "SelectCurrentType, nodeName={0}", nodeName );
            ConfigurationNode node = Section.SelectNode(nodeName, false);
            return node;
        }
    }
}