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
            var exists = xmlReader.MoveToFirstAttribute();

            while (exists)
            {
                var name = xmlReader.Name;
                var value = xmlReader.Value;
                var xmlAttribute = xmlDocument.CreateAttribute( name );
                xmlAttribute.Value = value;
                attributes.Append( xmlAttribute );
                exists = xmlReader.MoveToNextAttribute();
            }
        }

        private static bool MoveToElement( XmlReader xmlReader )
        {
            var found = false;

            while (xmlReader.Read())
            {
                var nodeType = xmlReader.NodeType;

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
            var found = MoveToElement( this.xmlReader );

            if (found)
            {
                var xmlDocument = new XmlDocument();
                var name = this.xmlReader.Name;
                xmlElement = xmlDocument.CreateElement( name );
                var attributes = xmlElement.Attributes;
                ReadAttributes( this.xmlReader, xmlDocument, attributes );
            }

            return xmlElement;
        }

        private static XmlElement ReadElement( XmlReader xmlReader, XmlDocument xmlDocument, int level )
        {
#if DEBUG
            var xmlLineInfo = xmlReader as IXmlLineInfo;

            if (xmlLineInfo != null)
            {
                Trace.WriteLine($"begin {level},{xmlReader.Name},{xmlLineInfo.LineNumber},{xmlLineInfo.LinePosition}");
            }
#endif

            var name = xmlReader.Name;
            var isEmptyElement = xmlReader.IsEmptyElement;
            var xmlElement = xmlDocument.CreateElement( name );

            if (xmlReader.HasAttributes)
            {
                ReadAttributes( xmlReader, xmlDocument, xmlElement.Attributes );
            }

            if (!isEmptyElement)
            {
                while (true)
                {
                    var read = xmlReader.Read();

                    if (!read)
                    {
                        break;
                    }

                    var nodeType = xmlReader.NodeType;
                    var breakable = false;

                    switch (nodeType)
                    {
                        case XmlNodeType.Text:
                            var value = xmlReader.Value;
                            var xmlText = xmlDocument.CreateTextNode( value );
                            xmlElement.AppendChild( xmlText );
                            break;

                        case XmlNodeType.CDATA:
                            var data = xmlReader.Value;
                            var xmlCDataSection = xmlDocument.CreateCDataSection( data );
                            xmlElement.AppendChild( xmlCDataSection );
                            break;

                        case XmlNodeType.Element:
                            var childElement = ReadElement( xmlReader, xmlDocument, level + 1 );
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
            var xmlLineInfo = this.xmlReader as IXmlLineInfo;

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
                var nodeType = this.xmlReader.NodeType;
                var breakable = false;

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