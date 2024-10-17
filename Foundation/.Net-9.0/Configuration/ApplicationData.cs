using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Foundation.Configuration;

public sealed class ApplicationData
{
    private string? _fileName;
    private string? _sectionName;

    public ConfigurationNode? RootNode { get; private set; }

    public ConfigurationNode CurrentNamespace
    {
        get
        {
            var trace = new StackTrace(1);
            var nodeName = ConfigurationNodeName.FromNamespace(trace, 0);
            var node = CreateNode(nodeName);
            return node;
        }
    }

    public ConfigurationNode CurrentType
    {
        get
        {
            var trace = new StackTrace(1);
            var nodeName = ConfigurationNodeName.FromType(trace, 0);
            var node = CreateNode(nodeName);
            return node;
        }
    }

    public static string GetApplicationDataFolderPath(bool versioned)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        var assembly = Assembly.GetEntryAssembly()!;
        var company = GetCompany(assembly);
        var product = GetProduct(assembly);
        var name = assembly.GetName();

        if (product == null)
            product = name.Name;

        if (company != null)
        {
            stringBuilder.Append(Path.DirectorySeparatorChar);
            stringBuilder.Append(company);
        }

        stringBuilder.Append(Path.DirectorySeparatorChar);
        stringBuilder.Append(product);

        if (versioned)
        {
            stringBuilder.Append(" (");
            stringBuilder.Append(name.Version);
            stringBuilder.Append(')');
        }

        return stringBuilder.ToString();
    }

    private static string? GetProduct(Assembly assembly)
    {
        string? product;
        var productAttribute = (AssemblyProductAttribute?)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute));

        if (productAttribute != null)
        {
            product = productAttribute.Product;

            if (product.Length == 0)
                product = null;
        }
        else
            product = null;

        return product;
    }

    private static string? GetCompany(Assembly assembly)
    {
        string? company;
        var companyAttribute = (AssemblyCompanyAttribute?)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute));

        if (companyAttribute != null)
        {
            company = companyAttribute.Company;

            if (company.Length == 0)
                company = null;
        }
        else
            company = null;

        return company;
    }

    public void Load(XmlReader xmlReader)
    {
        var reader = new ConfigurationReader();
        RootNode = reader.Read(xmlReader, _sectionName, null, null);

        if (RootNode == null)
            RootNode = new ConfigurationNode(null);
    }

    public void Load(string fileName, string sectionName)
    {
        _fileName = fileName;
        _sectionName = sectionName;

        if (File.Exists(fileName))
        {
            var reader = new ConfigurationReader();
            StringCollection fileNames = [];
            RootNode = reader.Read(fileName, sectionName, fileNames);
        }
        else
            RootNode = new ConfigurationNode(null);
    }

    public void Save(XmlWriter xmlWriter, string sectionName)
    {
        ArgumentNullException.ThrowIfNull(xmlWriter);
        ArgumentNullException.ThrowIfNull(sectionName);

        xmlWriter.WriteStartElement(sectionName);
        ConfigurationWriter.Write(xmlWriter, RootNode!.Attributes);

        foreach (var childNode in RootNode.ChildNodes)
            ConfigurationWriter.WriteNode(xmlWriter, childNode);

        xmlWriter.WriteEndElement();
    }

    public void Save(string fileName, string sectionName)
    {
        var directoryName = Path.GetDirectoryName(fileName)!;
        Directory.CreateDirectory(directoryName);

        using var xmlTextWriter = new XmlTextWriter(fileName, Encoding.UTF8);
        xmlTextWriter.Formatting = Formatting.Indented;
        xmlTextWriter.Indentation = 2;
        xmlTextWriter.IndentChar = ' ';
        xmlTextWriter.WriteStartDocument();
        Save(xmlTextWriter, sectionName);
        xmlTextWriter.WriteEndDocument();
        xmlTextWriter.Close();
    }

    public void Save()
    {
        var directoryName = Path.GetDirectoryName(_fileName)!;

        if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);

        Save(_fileName!, _sectionName!);
    }

    public ConfigurationNode CreateNode(string nodeName) => RootNode!.CreateNode(nodeName);
}