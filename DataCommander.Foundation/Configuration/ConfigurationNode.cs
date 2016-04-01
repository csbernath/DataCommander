namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ConfigurationNode
    {
        /// <summary>
        /// The path delimiter in the nodeName. E.g.: Node1/Node2/Node3.
        /// </summary>
        public const Char Delimiter = '/';

        private int index;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public ConfigurationNode(string name)
        {
            this.Name = name;
            this.HasName = name != null;
        }

        /// <summary>
        /// 
        /// </summary>
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

                if (this.Parent != null)
                {
                    fullName = this.Parent.FullName;

                    if (fullName != null)
                    {
                        fullName += Delimiter;
                    }

                    fullName += this.Name;
                }
                else
                {
                    fullName = null;
                }

                return fullName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childNode"></param>
        public void AddChildNode(ConfigurationNode childNode)
        {
            Contract.Requires<ArgumentException>(childNode.Parent == null);

            if (childNode.Name == null)
            {
                childNode.Name = ConfigurationElementName.Node + "[" + this.index + ']';
                this.index++;
            }

            this.ChildNodes.Add(childNode);
            childNode.Parent = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="childNode"></param>
        public void InsertChildNode(int index, ConfigurationNode childNode)
        {
            Contract.Requires<ArgumentException>(childNode.Parent == null);

            if (childNode.Name == null)
            {
                childNode.Name = ConfigurationElementName.Node + "[" + index + ']';
                index++;
            }

            this.ChildNodes.Insert(index, childNode);
            childNode.Parent = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childNode"></param>
        public void RemoveChildNode(ConfigurationNode childNode)
        {
            Contract.Requires(childNode != null);
            Contract.Requires(this == childNode.Parent);

            this.ChildNodes.Remove(childNode);
            childNode.Parent = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConfigurationNode Clone()
        {
            ConfigurationNode clone = new ConfigurationNode(this.Name);

            foreach (ConfigurationAttribute attribute in this.Attributes)
            {
                ConfigurationAttribute attributeClone = attribute.Clone();
                clone.Attributes.Add(attributeClone);
            }

            foreach (ConfigurationNode childNode in this.ChildNodes)
            {
                ConfigurationNode childNodeClone = childNode.Clone();
                clone.AddChildNode(childNodeClone);
            }

            return clone;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public ConfigurationNode CreateNode(string nodeName)
        {
            Contract.Requires(nodeName != null);
            ConfigurationNode node = this;
            string[] nodeNames = nodeName.Split(Delimiter);

            for (int i = 0; i < nodeNames.Length; i++)
            {
                ConfigurationNode childNode;
                bool contains = node.ChildNodes.TryGetValue(nodeNames[i], out childNode);

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
                    ConfigurationNode childNode;
                    bool contains = node.ChildNodes.TryGetValue(childNodeName, out childNode);

                    if (contains)
                    {
                        node = childNode;
                        depth++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (depth != childNodeNames.Length)
                {
                    node = null;
                }
            }

            return node;
        }

        /// <summary>
        /// Gets the attributes stored in this node.
        /// </summary>
        public ConfigurationAttributeCollection Attributes { get; } = new ConfigurationAttributeCollection();

        /// <summary>
        /// Gets the child nodes of this node.
        /// </summary>
        public ConfigurationNodeCollection ChildNodes { get; } = new ConfigurationNodeCollection();

        /// <summary>
        /// Writes the content of this node (attributes and child nodes)
        /// of this node to the specified <paramref name="textWriter"/>.
        /// </summary>
        /// <param name="textWriter"></param>
        public void Write(TextWriter textWriter)
        {
            textWriter.WriteLine("[" + this.FullName + "]");

            foreach (ConfigurationAttribute attribute in this.Attributes)
            {
                attribute.Write(textWriter);
            }

            textWriter.WriteLine();

            foreach (ConfigurationNode childNode in this.ChildNodes)
            {
                childNode.Write(textWriter);
            }
        }

        /// <summary>
        /// Writes the documentation of this node to the specified <paramref name="textWriter" />.
        /// </summary>
        /// <param name="textWriter"></param>
        /// <param name="level">Recursion level</param>
        public void WriteDocumentation(TextWriter textWriter, int level)
        {
            Contract.Requires(textWriter != null);

            StringBuilder sb = new StringBuilder();
            string indent = new string(' ', level * 2);
            sb.Append(indent);
            sb.Append(this.Name);
            sb.Append("\t\t");
            sb.AppendLine(this.Description);

            if (this.Attributes.Count > 0)
            {
                foreach (ConfigurationAttribute attribute in this.Attributes)
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

            foreach (ConfigurationNode childNode in this.ChildNodes)
            {
                childNode.WriteDocumentation(textWriter, level + 1);
            }
        }
    }
}