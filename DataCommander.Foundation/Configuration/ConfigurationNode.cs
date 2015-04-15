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

        private string name;
        private readonly bool hasName;
        private string description;
        private ConfigurationNode parent;
        private readonly ConfigurationNodeCollection childNodes = new ConfigurationNodeCollection();
        private readonly ConfigurationAttributeCollection attributes = new ConfigurationAttributeCollection();
        private int index;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public ConfigurationNode(string name)
        {
            this.name = name;
            this.hasName = name != null;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasName
        {
            get
            {
                return this.hasName;
            }
        }

        /// <summary>
        /// Gets the name of the node.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets/sets the description of the node.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        public ConfigurationNode Parent
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>
        /// Gets the full path of the node.
        /// </summary>
        public string FullName
        {
            get
            {
                string fullName;

                if (this.parent != null)
                {
                    fullName = this.parent.FullName;

                    if (fullName != null)
                    {
                        fullName += Delimiter;
                    }

                    fullName += this.name;
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

            if (childNode.name == null)
            {
                childNode.name = ConfigurationElementName.Node + "[" + this.index + ']';
                this.index++;
            }

            this.childNodes.Add(childNode);
            childNode.parent = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="childNode"></param>
        public void InsertChildNode(int index, ConfigurationNode childNode)
        {
            Contract.Requires<ArgumentException>(childNode.Parent == null);

            if (childNode.name == null)
            {
                childNode.name = ConfigurationElementName.Node + "[" + index + ']';
                index++;
            }

            this.childNodes.Insert(index, childNode);
            childNode.parent = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childNode"></param>
        public void RemoveChildNode(ConfigurationNode childNode)
        {
            Contract.Requires(childNode != null);
            Contract.Requires(this == childNode.Parent);

            this.childNodes.Remove(childNode);
            childNode.parent = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConfigurationNode Clone()
        {
            ConfigurationNode clone = new ConfigurationNode(this.name);

            foreach (ConfigurationAttribute attribute in this.attributes)
            {
                ConfigurationAttribute attributeClone = attribute.Clone();
                clone.Attributes.Add(attributeClone);
            }

            foreach (ConfigurationNode childNode in this.childNodes)
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
                    bool contains = node.childNodes.TryGetValue(childNodeName, out childNode);

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
        public ConfigurationAttributeCollection Attributes
        {
            get
            {
                return this.attributes;
            }
        }

        /// <summary>
        /// Gets the child nodes of this node.
        /// </summary>
        public ConfigurationNodeCollection ChildNodes
        {
            get
            {
                return this.childNodes;
            }
        }

        /// <summary>
        /// Writes the content of this node (attributes and child nodes)
        /// of this node to the specified <paramref name="textWriter"/>.
        /// </summary>
        /// <param name="textWriter"></param>
        public void Write(TextWriter textWriter)
        {
            textWriter.WriteLine("[" + this.FullName + "]");

            foreach (ConfigurationAttribute attribute in this.attributes)
            {
                attribute.Write(textWriter);
            }

            textWriter.WriteLine();

            foreach (ConfigurationNode childNode in this.childNodes)
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
            sb.Append(this.name);
            sb.Append("\t\t");
            sb.AppendLine(this.description);

            if (this.attributes.Count > 0)
            {
                foreach (ConfigurationAttribute attribute in this.attributes)
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

            foreach (ConfigurationNode childNode in this.childNodes)
            {
                childNode.WriteDocumentation(textWriter, level + 1);
            }
        }
    }
}