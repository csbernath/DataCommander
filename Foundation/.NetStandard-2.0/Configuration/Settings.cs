using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Foundation.Assertions;

namespace Foundation.Configuration
{
    /// <summary>
    /// This class is a singleton which holds a <see cref="ConfigurationSection"/> instance.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// The config file name.
        /// </summary>
        private static string _configFileName;

        private static string _sectionName;

        /// <summary>
        /// The ConfigurationSection instance.
        /// </summary>
        private static ConfigurationSection _section;

        public static event EventHandler Changed
        {
            add => Section.Changed += value;
            remove => Section.Changed -= value;
        }

        /// <summary>
        /// Uses <see cref="AppSettings"/> to retrieve "ConfigFileName" from the app.config file.
        /// </summary>
        public static string ConfigFileName
        {
            get
            {
                if (_configFileName == null)
                {
#if FOUNDATION_4_7
                    var setup = AppDomain.CurrentDomain.SetupInformation;
                    bool contains;

                    try
                    {
                        contains = AppSettings.CurrentType.TryGetString("ConfigFileName", out _configFileName);
                    }
                    catch
                    {
                        contains = false;
                    }

                    if (contains && !string.IsNullOrEmpty(_configFileName))
                        _configFileName = Path.Combine(setup.ApplicationBase, _configFileName);
                    else
                        _configFileName = setup.ConfigurationFile;
#else
                    var domain = AppDomain.CurrentDomain;
                    _configFileName = $"{domain.BaseDirectory}{domain.FriendlyName}.config";
#endif
                }

                return _configFileName;
            }

            set => _configFileName = value;
        }

        public static ConfigurationNode RootNode => Section.RootNode;

        /// <summary>
        /// Gets the <see cref="ConfigurationSection" /> instance.
        /// Initializes the instance at first call.
        /// </summary>
        public static ConfigurationSection Section
        {
            get
            {
                if (_section == null)
                {
                    var configFileName = ConfigFileName;
                    var sectionName = SectionName;
                    _section = new ConfigurationSection(configFileName, sectionName);
                }

                return _section;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string SectionName
        {
            get
            {
                if (_sectionName == null)
                    _sectionName = ConfigurationSection.DefaultSectionName;

                return _sectionName;
            }

            set => _sectionName = value;
        }

        public static ConfigurationNode CurrentMethod
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                var trace = new StackTrace(1);
                var nodeName = ConfigurationNodeName.FromMethod(trace, 0);
                var node = Section.SelectNode(nodeName, true);
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
                var nodeName = ConfigurationNodeName.FromType(trace, 0);
                var node = Section.SelectNode(nodeName, true);
                return node;
            }
        }

        public static ConfigurationNode CurrentNamespace
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                var trace = new StackTrace(1);
                var nodeName = ConfigurationNodeName.FromNamespace(trace, 0);
                var node = Section.SelectNode(nodeName, true);
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
            Assert.IsNotNull(assembly);

            var codeBase = assembly.CodeBase;
            var uri = new Uri(codeBase);
            var fileName = uri.LocalPath;
            var fileInfo = new FileInfo(fileName);
            fileName = fileInfo.FullName;
            var configFilename = fileName + ".config";
            return configFilename;
        }

        public static ConfigurationNode SelectNode(string nodeName, bool throwOnError) => Section.SelectNode(nodeName, throwOnError);

        internal static ConfigurationNode SelectNode(Type type, bool throwOnError)
        {
            var nodeName = ConfigurationNodeName.FromType(type);
            var node = Section.SelectNode(nodeName, throwOnError);
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
            var nodeName = ConfigurationNodeName.FromType(trace, 0);
            //log.Trace( "SelectCurrentType, nodeName={0}", nodeName );
            var node = Section.SelectNode(nodeName, false);
            return node;
        }
    }
}