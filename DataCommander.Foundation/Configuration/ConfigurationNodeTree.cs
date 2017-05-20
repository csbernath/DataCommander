namespace DataCommander.Foundation.Configuration
{
    using System.Globalization;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ConfigurationNodeTree
    {
        private ConfigurationNode _rootNode = new ConfigurationNode(null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="sectionName"></param>
        public void LoadXml(string xml, string sectionName)
        {
            var reader = new ConfigurationReader();
            var textReader = new StringReader(xml);
            var xmlReader = new XmlTextReader(textReader);
            this._rootNode = reader.Read(xmlReader, null, sectionName, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="sectionName"></param>
        public void Save(XmlWriter xmlWriter, string sectionName)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(xmlWriter != null);
            Contract.Requires<ArgumentNullException>(sectionName != null);
#endif

            xmlWriter.WriteStartElement(sectionName);
            ConfigurationWriter.Write(xmlWriter, this._rootNode.Attributes);

            foreach (var childNode in this._rootNode.ChildNodes)
            {
                ConfigurationWriter.WriteNode(xmlWriter, childNode);
            }

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public string GetXml(string sectionName)
        {
            string s;

            using (var textWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                var xmlWriter = new XmlTextWriter(textWriter) {Formatting = Formatting.Indented, Indentation = 2, IndentChar = ' '};
                this.Save(xmlWriter, sectionName);
                xmlWriter.Close();
                s = textWriter.ToString();
            }

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ConfigurationNode SelectNode(string path)
        {
            if (this._rootNode == null)
            {
                this._rootNode = new ConfigurationNode(null);
            }

            var node = this._rootNode;

            if (path != null)
            {
                var nodeNames = path.Split(ConfigurationNode.Delimiter);

                for (var i = 0; i < nodeNames.Length; i++)
                {
                    var childNodeName = nodeNames[i];
                    ConfigurationNode childNode;
                    var contains = node.ChildNodes.TryGetValue(childNodeName, out childNode);

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
}