namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ConfigurationNodeTree
    {
        private ConfigurationNode rootNode = new ConfigurationNode(null);

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
            this.rootNode = reader.Read(xmlReader, null, sectionName, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="sectionName"></param>
        public void Save(XmlWriter xmlWriter, string sectionName)
        {
            Contract.Requires<ArgumentNullException>(xmlWriter != null);
            Contract.Requires<ArgumentNullException>(sectionName != null);

            xmlWriter.WriteStartElement(sectionName);
            ConfigurationWriter.Write(xmlWriter, this.rootNode.Attributes);

            foreach (ConfigurationNode childNode in this.rootNode.ChildNodes)
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
            if (this.rootNode == null)
            {
                this.rootNode = new ConfigurationNode(null);
            }

            ConfigurationNode node = this.rootNode;

            if (path != null)
            {
                string[] nodeNames = path.Split(ConfigurationNode.Delimiter);

                for (int i = 0; i < nodeNames.Length; i++)
                {
                    string childNodeName = nodeNames[i];
                    ConfigurationNode childNode;
                    bool contains = node.ChildNodes.TryGetValue(childNodeName, out childNode);

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