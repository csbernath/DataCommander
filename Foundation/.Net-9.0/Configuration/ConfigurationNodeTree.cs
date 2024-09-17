using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Foundation.Configuration;

public sealed class ConfigurationNodeTree
{
    private ConfigurationNode _rootNode = new(null);

    public void LoadXml(string xml, string sectionName)
    {
        ConfigurationReader reader = new ConfigurationReader();
        StringReader textReader = new StringReader(xml);
        XmlTextReader xmlReader = new XmlTextReader(textReader);
        _rootNode = reader.Read(xmlReader, null, sectionName, null);
    }

    public void Save(XmlWriter xmlWriter, string sectionName)
    {
        ArgumentNullException.ThrowIfNull(xmlWriter);
        ArgumentNullException.ThrowIfNull(sectionName);

        xmlWriter.WriteStartElement(sectionName);
        ConfigurationWriter.Write(xmlWriter, _rootNode.Attributes);

        foreach (ConfigurationNode childNode in _rootNode.ChildNodes)
        {
            ConfigurationWriter.WriteNode(xmlWriter, childNode);
        }

        xmlWriter.WriteEndElement();
    }

    public string GetXml(string sectionName)
    {
        string s;

        using (StringWriter textWriter = new StringWriter(CultureInfo.InvariantCulture))
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(textWriter) {Formatting = Formatting.Indented, Indentation = 2, IndentChar = ' '};
            Save(xmlWriter, sectionName);
            xmlWriter.Close();
            s = textWriter.ToString();
        }

        return s;
    }

    public ConfigurationNode SelectNode(string path)
    {
        if (_rootNode == null)
            _rootNode = new ConfigurationNode(null);

        ConfigurationNode node = _rootNode;

        if (path != null)
        {
            string[] nodeNames = path.Split(ConfigurationNode.Delimiter);

            for (int i = 0; i < nodeNames.Length; i++)
            {
                string childNodeName = nodeNames[i];
                bool contains = node.ChildNodes.TryGetValue(childNodeName, out ConfigurationNode childNode);

                if (!contains)
                {
                    childNode = new ConfigurationNode(childNodeName);
                    node.AddChildNode(childNode);
                }

                node = childNode;
            }
        }

        return node;
    }
}