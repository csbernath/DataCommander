namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Name = {Name}, Value = {Value}, Description = {Description}")]
    public sealed class ConfigurationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        public ConfigurationAttribute(
            string name,
            object value,
            string description)
        {
            this.Name = name;
            this.Value = value;
            this.Description = description;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>()
        {
            Contract.Requires((this.Value == null && typeof (T).IsClass) || this.Value is T);

            T value = (T)this.Value;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConfigurationAttribute Clone()
        {
            var clone = new ConfigurationAttribute(this.Name, this.Value, this.Description);
            return clone;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        public void Write(TextWriter textWriter)
        {
            string typeName;
            Type type = null;

            if (this.Value != null)
            {
                type = this.Value.GetType();
                typeName = TypeNameCollection.GetTypeName(type);
            }
            else
            {
                typeName = "object";
            }

            textWriter.Write("  " + typeName + " " + this.Name + " = ");

            if (type != null)
            {
                if (type.IsArray)
                {
                    Type elementType = type.GetElementType();

                    if (elementType == typeof (byte))
                    {
                        byte[] inArray = (byte[])this.Value;
                        string base64 = System.Convert.ToBase64String(inArray);
                        textWriter.WriteLine(base64);
                    }
                    else
                    {
                        Array array = (Array)this.Value;

                        if (array.Length > 0)
                        {
                            textWriter.WriteLine();

                            int index = 0;

                            foreach (object arrayItem in array)
                            {
                                textWriter.Write("    [" + index + "] = ");
                                Write(arrayItem, textWriter);
                                index++;
                            }
                        }
                    }
                }
                else
                {
                    Write(this.Value, textWriter);
                }
            }
            else
            {
                Write(this.Value, textWriter);
            }
        }

        [ContractVerification(false)]
        private static void Write(object attributeValue, TextWriter textWriter)
        {
            string attibuteValueString;

            if (attributeValue != null)
            {
                Type type = attributeValue.GetType();
                TypeCode typeCode = Type.GetTypeCode(type);

                switch (typeCode)
                {
                    case TypeCode.String:
                        attibuteValueString = "\"" + attributeValue + "\"";
                        break;

                    case TypeCode.Object:
                    {
                        if (type == typeof (TimeSpan))
                        {
                            attibuteValueString = attributeValue.ToString();
                        }
                        else
                        {
                            XmlNode xmlNode = attributeValue as XmlNode;

                            if (xmlNode != null)
                            {
                                attibuteValueString = xmlNode.OuterXml;
                            }
                            else
                            {
                                xmlNode = XmlHelper.Serialize(attributeValue);
                                attibuteValueString = xmlNode.OuterXml;
                            }
                        }
                    }

                        break;

                    default:
                        attibuteValueString = attributeValue.ToString();
                        break;
                }
            }
            else
            {
                attibuteValueString = "null";
            }

            textWriter.WriteLine(attibuteValueString);
        }

        /// <summary>
        /// 
        /// </summary>
        private static class XmlHelper
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>        
            public static XmlElement Serialize(object obj)
            {
                Contract.Requires<ArgumentNullException>(obj != null);

                Type type = obj.GetType();
                var xmlSerializer = new XmlSerializer(type);
                StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
                xmlSerializer.Serialize(stringWriter, obj);

                StringBuilder sb = stringWriter.GetStringBuilder();
                string s = sb.ToString();

                var xmlDocument = new XmlDocument();
                xmlDocument.InnerXml = s;
                XmlElement xmlElement = xmlDocument.DocumentElement;

                // removes unnecessary attributes generated by XmlSerializer
                xmlElement.Attributes.RemoveAll();

                return xmlElement;
            }
        }
    }
}