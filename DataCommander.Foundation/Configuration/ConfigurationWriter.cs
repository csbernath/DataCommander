namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using DataCommander.Foundation.Xml;

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
            string xmlElementName;
            string xmlAttributeValue;

            if (node.HasName)
            {
                string nodeName = node.Name;
                string encodedName = XmlConvert.EncodeName(nodeName);

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

            using (xmlWriter.WriteElement(xmlElementName))
            {

                if (xmlAttributeValue != null)
                {
                    xmlWriter.WriteAttributeString("name", xmlAttributeValue);
                }

                Write(xmlWriter, node.Attributes);

                foreach (ConfigurationNode childNode in node.ChildNodes)
                {
                    WriteNode(xmlWriter, childNode);
                }
            }
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
                using (xmlWriter.WriteElement(ConfigurationElementName.Attribute))
                {
                    xmlWriter.WriteAttributeString("name", attribute.Name);
                    object value = attribute.Value;

                    if (value != null)
                    {
                        Type type = value.GetType();

                        if (type != typeof (string))
                        {
                            string typeName = TypeNameCollection.GetTypeName(type);
                            xmlWriter.WriteAttributeString("type", typeName);
                        }

                        TypeCode typeCode = Type.GetTypeCode(type);
                        string strValue;

                        switch (typeCode)
                        {
                            case TypeCode.Object:
                                if (type == typeof (TimeSpan))
                                {
                                    TimeSpan timeSpan = (TimeSpan) value;
                                    strValue = timeSpan.ToString();
                                    xmlWriter.WriteAttributeString("value", strValue);
                                }
                                else if (type.IsArray)
                                {
                                    Array array = (Array) value;

                                    for (int j = 0; j < array.Length; j++)
                                    {
                                        using (xmlWriter.WriteElement("a"))
                                        {
                                            value = array.GetValue(j);
                                            strValue = value.ToString();
                                            xmlWriter.WriteAttributeString("value", strValue);
                                        }
                                    }
                                }
                                else
                                {
                                    var xmlSerializer = new XmlSerializer(type);
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
                        xmlWriter.WriteAttributeString("isNull", bool.TrueString);
                    }
                }
            }
        }
    }
}