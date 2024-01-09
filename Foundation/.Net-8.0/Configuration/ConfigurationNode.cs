using System;
using System.IO;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Configuration;

public sealed class ConfigurationNode
{
    /// <summary>
    /// The path delimiter in the nodeName. E.g.: Node1/Node2/Node3.
    /// </summary>
    public const char Delimiter = '/';

    private int _index;

    public ConfigurationNode(string name)
    {
        Name = name;
        HasName = name != null;
    }

    public bool HasName { get; }

    /// <summary>
    /// Gets the name of the node.
    /// </summary>
    public string Name { get; private set; }

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
        var clone = new ConfigurationNode(Name);

        foreach (var attribute in Attributes)
        {
            var attributeClone = attribute.Clone();
            clone.Attributes.Add(attributeClone);
        }

        foreach (var childNode in ChildNodes)
        {
            var childNodeClone = childNode.Clone();
            clone.AddChildNode(childNodeClone);
        }

        return clone;
    }

    public ConfigurationNode CreateNode(string nodeName)
    {
        ArgumentNullException.ThrowIfNull(nodeName);

        var node = this;
        var nodeNames = nodeName.Split(Delimiter);

        for (var i = 0; i < nodeNames.Length; i++)
        {
            var contains = node.ChildNodes.TryGetValue(nodeNames[i], out var childNode);
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
        var node = this;

        if (path != null)
        {
            var childNodeNames = path.Split(Delimiter);
            var depth = 0;

            foreach (var childNodeName in childNodeNames)
            {
                var contains = node.ChildNodes.TryGetValue(childNodeName, out var childNode);

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

        foreach (var attribute in Attributes)
            attribute.Write(textWriter);

        textWriter.WriteLine();

        foreach (var childNode in ChildNodes)
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

        var sb = new StringBuilder();
        var indent = new string(' ', level * 2);
        sb.Append(indent);
        sb.Append(Name);
        sb.Append("\t\t");
        sb.AppendLine(Description);

        if (Attributes.Count > 0)
        {
            foreach (var attribute in Attributes)
            {
                sb.Append('\t');
                sb.Append(attribute.Name);

                sb.Append('\t');
                sb.Append(attribute.Description);
                sb.Append('\t');

                var value = attribute.Value;
                var valueString = value != null ? value.ToString() : null;
                var multiline = valueString.IndexOf('\n') >= 0;

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

        foreach (var childNode in ChildNodes)
        {
            childNode.WriteDocumentation(textWriter, level + 1);
        }
    }
}