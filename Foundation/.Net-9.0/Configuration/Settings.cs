using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Foundation.Configuration;

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
                string location = Assembly.GetEntryAssembly().Location;
                _configFileName = $"{location}.config";
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
                string configFileName = ConfigFileName;
                string sectionName = SectionName;
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
            StackTrace trace = new StackTrace(1);
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
            StackTrace trace = new StackTrace(1);
            string nodeName = ConfigurationNodeName.FromType(trace, 0);
            ConfigurationNode node = Section.SelectNode(nodeName, true);
            return node;
        }
    }

    public static ConfigurationNode CurrentNamespace
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get
        {
            StackTrace trace = new StackTrace(1);
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
        ArgumentNullException.ThrowIfNull(assembly);

        string location = assembly.Location;
        Uri uri = new Uri(location);
        string fileName = uri.LocalPath;
        FileInfo fileInfo = new FileInfo(fileName);
        fileName = fileInfo.FullName;
        string configFilename = fileName + ".config";
        return configFilename;
    }

    public static ConfigurationNode SelectNode(string nodeName, bool throwOnError) => Section.SelectNode(nodeName, throwOnError);

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
        StackTrace trace = new StackTrace(1);
        string nodeName = ConfigurationNodeName.FromType(trace, 0);
        //log.Trace( "SelectCurrentType, nodeName={0}", nodeName );
        ConfigurationNode node = Section.SelectNode(nodeName, false);
        return node;
    }
}