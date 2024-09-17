using System;
using System.IO;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Configuration;

public sealed class ConfigurationNode(string name)
{
    /// <summary>
    /// The path delimiter in the nodeName. E.g.: Node1/Node2/Node3.
    /// </summary>
    public const char Delimiter = '/';

    private int _index;

    public bool HasName { get; } = name != null;

    /// <summary>
    /// Gets the name of the node.
    /// </summary>
    public string Name { get; private set; } = name;

    /// <summary>
    /// Gets/sets the description of the node.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets the parent node.
    /// </summary>
    public ConfigurationNode Parent { get; private set; }

    /// <summary>
    /// Gets the full path of the node.
    /// </summary>
    public string FullName
    {
        get
        {
            string fullName;

            if (Parent != null)
            {
                fullName = Parent.FullName;

                if (fullName != null)
                    fullName += Delimiter;

                fullName += Name;
            }
            else
                fullName = null;

            return fullName;
        }
    }

    public void AddChildNode(ConfigurationNode childNode)
    {
        Assert.IsTrue(childNode.Parent == null);

        if (childNode.Name == null)
        {
            childNode.Name = ConfigurationElementName.Node + "[" + _index + ']';
            _index++;
        }

        ChildNodes.Add(childNode);
        childNode.Parent = this;
    }

    public void InsertChildNode(int index, ConfigurationNode childNode)
    {
        Assert.IsTrue(childNode.Parent == null);

        if (childNode.Name == null)
        {
            childNode.Name = ConfigurationElementName.Node + "[" + index + ']';
            index++;
        }

        ChildNodes.Insert(index, childNode);
        childNode.Parent = this;
    }

    public void RemoveChildNode(ConfigurationNode childNode)
    {
        ArgumentNullException.ThrowIfNull(childNode);
        Assert.IsValidOperation(this == childNode.Parent);

        ChildNodes.Remove(childNode);
        childNode.Parent = null;
    }

    public ConfigurationNode Clone()
    {
        ConfigurationNode clone = new ConfigurationNode(Name);

        foreach (ConfigurationAttribute attribute in Attributes)
        {
            ConfigurationAttribute attributeClone = attribute.Clone();
            clone.Attributes.Add(attributeClone);
        }

        foreach (ConfigurationNode childNode in ChildNodes)
        {
            ConfigurationNode childNodeClone = childNode.Clone();
            clone.AddChildNode(childNodeClone);
        }

        return clone;
    }

    public ConfigurationNode CreateNode(string nodeName)
    {
        ArgumentNullException.ThrowIfNull(nodeName);

        ConfigurationNode node = this;
        string[] nodeNames = nodeName.Split(Delimiter);

        for (int i = 0; i < nodeNames.Length; i++)
        {
            bool contains = node.ChildNodes.TryGetValue(nodeNames[i], out ConfigurationNode childNode);
            if (!contains)
            {
                childNode = new ConfigurationNode(nodeNames[i]);
                node.AddChildNode(childNode);
            }

            node = childNode;
        }

        return node;
    }

    /// <summary>
    /// Finds recursively a node under the node.
    /// </summary>
    /// <param name="path">Name of the child node.
    /// The name can contains path delimiters.</param>
    /// <returns>Return the child node is found.
    /// Returns null if no child node found.</returns>
    public ConfigurationNode SelectNode(string path)
    {
        ConfigurationNode node = this;

        if (path != null)
        {
            string[] childNodeNames = path.Split(Delimiter);
            int depth = 0;

            foreach (string childNodeName in childNodeNames)
            {
                bool contains = node.ChildNodes.TryGetValue(childNodeName, out ConfigurationNode childNode);

                if (contains)
                {
                    node = childNode;
                    depth++;
                }
                else
                    break;
            }

            if (depth != childNodeNames.Length)
                node = null;
        }

        return node;
    }

    /// <summary>
    /// Gets the attributes stored in this node.
    /// </summary>
    public ConfigurationAttributeCollection Attributes { get; } = [];

    /// <summary>
    /// Gets the child nodes of this node.
    /// </summary>
    public ConfigurationNodeCollection ChildNodes { get; } = [];

    /// <summary>
    /// Writes the content of this node (attributes and child nodes)
    /// of this node to the specified <paramref name="textWriter"/>.
    /// </summary>
    /// <param name="textWriter"></param>
    public void Write(TextWriter textWriter)
    {
        textWriter.WriteLine("[" + FullName + "]");

        foreach (ConfigurationAttribute attribute in Attributes)
            attribute.Write(textWriter);

        textWriter.WriteLine();

        foreach (ConfigurationNode childNode in ChildNodes)
            childNode.Write(textWriter);
    }

    /// <summary>
    /// Writes the documentation of this node to the specified <paramref name="textWriter" />.
    /// </summary>
    /// <param name="textWriter"></param>
    /// <param name="level">Recursion level</param>
    public void WriteDocumentation(TextWriter textWriter, int level)
    {
        ArgumentNullException.ThrowIfNull(textWriter);

        StringBuilder sb = new StringBuilder();
        string indent = new string(' ', level * 2);
        sb.Append(indent);
        sb.Append(Name);
        sb.Append("\t\t");
        sb.AppendLine(Description);

        if (Attributes.Count > 0)
        {
            foreach (ConfigurationAttribute attribute in Attributes)
            {
                sb.Append('\t');
                sb.Append(attribute.Name);

                sb.Append('\t');
                sb.Append(attribute.Description);
                sb.Append('\t');

                object value = attribute.Value;
                string valueString = value != null ? value.ToString() : null;
                bool multiline = valueString.IndexOf('\n') >= 0;

                if (multiline)
                {
                    value = valueString.Replace("\r", string.Empty);
                    sb.Append('"');
                }

                sb.Append(value);

                if (multiline)
                {
                    sb.Append('"');
                }

                sb.Append(Environment.NewLine);
            }
        }

        textWriter.Write(sb);

        foreach (ConfigurationNode childNode in ChildNodes)
        {
            childNode.WriteDocumentation(textWriter, level + 1);
        }
    }
}