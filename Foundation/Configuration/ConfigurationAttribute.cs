using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Foundation.Configuration
{
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
#if CONTRACTS_FULL
            Contract.Requires((this.Value == null && typeof (T).IsClass) || this.Value is T);
#endif

            var value = (T)this.Value;
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
                    var elementType = type.GetElementType();

                    if (elementType == typeof (byte))
                    {
                        var inArray = (byte[])this.Value;
                        var base64 = System.Convert.ToBase64String(inArray);
                        textWriter.WriteLine(base64);
                    }
                    else
                    {
                        var array = (Array)this.Value;

                        if (array.Length > 0)
                        {
                            textWriter.WriteLine();

                            var index = 0;

                            foreach (var arrayItem in array)
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

        //[ContractVerification(false)]
        private static void Write(object attributeValue, TextWriter textWriter)
        {
            string attibuteValueString;

            if (attributeValue != null)
            {
                var type = attributeValue.GetType();
                var typeCode = Type.GetTypeCode(type);

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
                            var xmlNode = attributeValue as XmlNode;

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
#if CONTRACTS_FULL
                Contract.Requires<ArgumentNullException>(obj != null);
#endif

                var type = obj.GetType();
                var xmlSerializer = new XmlSerializer(type);
                var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
                xmlSerializer.Serialize(stringWriter, obj);

                var sb = stringWriter.GetStringBuilder();
                var s = sb.ToString();

                var xmlDocument = new XmlDocument();
                xmlDocument.InnerXml = s;
                var xmlElement = xmlDocument.DocumentElement;

                // removes unnecessary attributes generated by XmlSerializer
                xmlElement.Attributes.RemoveAll();

                return xmlElement;
            }
        }
    }
}