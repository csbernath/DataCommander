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
        private ConfigurationNode rootNode = new ConfigurationNode( null );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="sectionName"></param>
        public void LoadXml( String xml, String sectionName )
        {
            var reader = new ConfigurationReader();
            var textReader = new StringReader( xml );
            XmlTextReader xmlReader = new XmlTextReader( textReader );
            this.rootNode = reader.Read( xmlReader, null, sectionName, null );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="sectionName"></param>
        public void Save( XmlWriter xmlWriter, String sectionName )
        {
            Contract.Requires( xmlWriter != null );
            Contract.Requires( sectionName != null );

            xmlWriter.WriteStartElement( sectionName );
            ConfigurationWriter.Write( xmlWriter, this.rootNode.Attributes );

            foreach (ConfigurationNode childNode in this.rootNode.ChildNodes)
            {
                ConfigurationWriter.WriteNode( xmlWriter, childNode );
            }

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public String GetXml( String sectionName )
        {
            String s;
            using (var textWriter = new StringWriter( CultureInfo.InvariantCulture ))
            {
                var xmlWriter = new XmlTextWriter( textWriter ) { Formatting = Formatting.Indented, Indentation = 2, IndentChar = ' ' };
                this.Save( xmlWriter, sectionName );
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
        public ConfigurationNode SelectNode( String path )
        {
            if (this.rootNode == null)
            {
                this.rootNode = new ConfigurationNode( null );
            }

            ConfigurationNode node = this.rootNode;

            if (path != null)
            {
                String[] nodeNames = path.Split( ConfigurationNode.Delimiter );

                for (Int32 i = 0; i < nodeNames.Length; i++)
                {
                    String childNodeName = nodeNames[ i ];
                    ConfigurationNode childNode;
                    Boolean contains = node.ChildNodes.TryGetValue( childNodeName, out childNode );

                    if (!contains)
                    {
                        childNode = new ConfigurationNode( childNodeName );
                        node.AddChildNode( childNode );
                    }

                    node = childNode;
                }
            }

            return node;
        }
    }
}