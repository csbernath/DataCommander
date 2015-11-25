namespace DataCommander.Foundation.Xml
{
    using System.Xml;
#if DEBUG
    using System.Diagnostics;
#endif

    /// <summary>
    /// 
    /// </summary>
    public class XmlElementReader
    {
        private readonly XmlReader xmlReader;
        private readonly XmlDocument xmlDocument = new XmlDocument();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlReader"></param>
        public XmlElementReader( XmlReader xmlReader )
        {
            this.xmlReader = xmlReader;
        }

        private static void ReadAttributes(
            XmlReader xmlReader,
            XmlDocument xmlDocument,
            XmlAttributeCollection attributes )
        {
            bool exists = xmlReader.MoveToFirstAttribute();

            while (exists)
            {
                string name = xmlReader.Name;
                string value = xmlReader.Value;
                XmlAttribute xmlAttribute = xmlDocument.CreateAttribute( name );
                xmlAttribute.Value = value;
                attributes.Append( xmlAttribute );
                exists = xmlReader.MoveToNextAttribute();
            }
        }

        private static bool MoveToElement( XmlReader xmlReader )
        {
            bool found = false;

            while (xmlReader.Read())
            {
                XmlNodeType nodeType = xmlReader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlElement ReadStartElement()
        {
            XmlElement xmlElement = null;
            bool found = MoveToElement( this.xmlReader );

            if (found)
            {
                XmlDocument xmlDocument = new XmlDocument();
                string name = this.xmlReader.Name;
                xmlElement = xmlDocument.CreateElement( name );
                XmlAttributeCollection attributes = xmlElement.Attributes;
                ReadAttributes( this.xmlReader, xmlDocument, attributes );
            }

            return xmlElement;
        }

        private static XmlElement ReadElement( XmlReader xmlReader, XmlDocument xmlDocument, int level )
        {
#if DEBUG
            IXmlLineInfo xmlLineInfo = xmlReader as IXmlLineInfo;

            if (xmlLineInfo != null)
            {
                Trace.WriteLine($"begin {level},{xmlReader.Name},{xmlLineInfo.LineNumber},{xmlLineInfo.LinePosition}");
            }
#endif

            string name = xmlReader.Name;
            bool isEmptyElement = xmlReader.IsEmptyElement;
            XmlElement xmlElement = xmlDocument.CreateElement( name );

            if (xmlReader.HasAttributes)
            {
                ReadAttributes( xmlReader, xmlDocument, xmlElement.Attributes );
            }

            if (!isEmptyElement)
            {
                while (true)
                {
                    bool read = xmlReader.Read();

                    if (!read)
                    {
                        break;
                    }

                    XmlNodeType nodeType = xmlReader.NodeType;
                    bool breakable = false;

                    switch (nodeType)
                    {
                        case XmlNodeType.Text:
                            string value = xmlReader.Value;
                            XmlText xmlText = xmlDocument.CreateTextNode( value );
                            xmlElement.AppendChild( xmlText );
                            break;

                        case XmlNodeType.CDATA:
                            string data = xmlReader.Value;
                            XmlCDataSection xmlCDataSection = xmlDocument.CreateCDataSection( data );
                            xmlElement.AppendChild( xmlCDataSection );
                            break;

                        case XmlNodeType.Element:
                            XmlElement childElement = ReadElement( xmlReader, xmlDocument, level + 1 );
                            xmlElement.AppendChild( childElement );
                            break;

                        case XmlNodeType.EndElement:
                            breakable = true;
                            break;

                        default:
                            break;
                    }

                    if (breakable)
                    {
                        break;
                    }
                }
            }

#if DEBUG
            if (xmlLineInfo != null)
            {
                Trace.WriteLine($"end {level},{xmlLineInfo.LineNumber},{xmlLineInfo.LinePosition}");
            }
#endif

            return xmlElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlElement ReadElement()
        {
#if DEBUG
            IXmlLineInfo xmlLineInfo = this.xmlReader as IXmlLineInfo;

            if (xmlLineInfo != null)
            {
                Trace.WriteLine($"BEGIN {xmlLineInfo.LineNumber},{xmlLineInfo.LinePosition}");
            }
#endif

            XmlElement xmlElement = null;

            while (this.xmlReader.Read())
            {
#if DEBUG
                Trace.WriteLine($"{this.xmlReader.Name},{this.xmlReader.NodeType}");
#endif
                XmlNodeType nodeType = this.xmlReader.NodeType;
                bool breakable = false;

                switch (nodeType)
                {
                    case XmlNodeType.Element:
                        xmlElement = ReadElement( this.xmlReader, this.xmlDocument, 0 );
                        breakable = true;
                        break;

                    case XmlNodeType.EndElement:
                        breakable = true;
                        break;

                    default:
                        break;
                }

                if (breakable)
                {
                    break;
                }
            }

#if DEBUG
            if (xmlLineInfo != null)
            {
                Trace.WriteLine($"END {xmlLineInfo.LineNumber},{xmlLineInfo.LinePosition}");
            }
#endif

            return xmlElement;
        }
    }
}