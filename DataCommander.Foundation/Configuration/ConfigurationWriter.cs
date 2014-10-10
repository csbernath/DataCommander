namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// 
    /// </summary>
    public static class ConfigurationWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="node"></param>
        public static void WriteNode(XmlWriter xmlWriter, ConfigurationNode node)
        {
            String xmlElementName;
            String xmlAttributeValue;

            if (node.HasName)
            {
                String nodeName = node.Name;
                String encodedName = XmlConvert.EncodeName(nodeName);

                if (nodeName == encodedName)
                {
                    xmlElementName = nodeName;
                    xmlAttributeValue = null;
                }
                else
                {
                    xmlElementName = ConfigurationElementName.Node;
                    xmlAttributeValue = nodeName;
                }
            }
            else
            {
                xmlElementName = ConfigurationElementName.Node;
                xmlAttributeValue = null;
            }

            xmlWriter.WriteStartElement(xmlElementName);

            if (xmlAttributeValue != null)
            {
                xmlWriter.WriteAttributeString("name", xmlAttributeValue);
            }

            Write(xmlWriter, node.Attributes);

            foreach (ConfigurationNode childNode in node.ChildNodes)
            {
                WriteNode(xmlWriter, childNode);
            }

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="attributes"></param>
        public static void Write(XmlWriter xmlWriter, ConfigurationAttributeCollection attributes)
        {
            foreach (ConfigurationAttribute attribute in attributes)
            {
                xmlWriter.WriteStartElement(ConfigurationElementName.Attribute);
                xmlWriter.WriteAttributeString("name", attribute.Name);
                Object value = attribute.Value;

                if (value != null)
                {
                    Type type = value.GetType();

                    if (type != typeof(String))
                    {
                        String typeName = TypeNameCollection.GetTypeName(type);
                        xmlWriter.WriteAttributeString("type", typeName);
                    }

                    TypeCode typeCode = Type.GetTypeCode(type);
                    String strValue;

                    switch (typeCode)
                    {
                        case TypeCode.Object:
                            if (type == typeof(TimeSpan))
                            {
                                TimeSpan timeSpan = (TimeSpan)value;
                                strValue = timeSpan.ToString();
                                xmlWriter.WriteAttributeString("value", strValue);
                            }
                            else if (type.IsArray)
                            {
                                Array array = (Array)value;

                                for (Int32 j = 0; j < array.Length; j++)
                                {
                                    xmlWriter.WriteStartElement("a");
                                    value = array.GetValue(j);
                                    strValue = value.ToString();
                                    xmlWriter.WriteAttributeString("value", strValue);
                                    xmlWriter.WriteEndElement();
                                }
                            }
                            else
                            {
                                XmlSerializer xmlSerializer = new XmlSerializer(type);
                                xmlSerializer.Serialize(xmlWriter, value);
                            }

                            break;

                        default:
                            strValue = value.ToString();
                            xmlWriter.WriteAttributeString("value", strValue);
                            break;
                    }
                }
                else
                {
                    xmlWriter.WriteAttributeString("isNull", Boolean.TrueString);
                }

                xmlWriter.WriteEndElement();
            }
        }
    }
}