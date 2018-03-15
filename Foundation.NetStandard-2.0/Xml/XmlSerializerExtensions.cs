using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Xml
{
    /// <summary>
    /// 
    /// </summary>
    public static class XmlSerializerExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlSerializer"></param>
        /// <param name="xmlWriterSettings"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string SerializeToXmlString(this XmlSerializer xmlSerializer, XmlWriterSettings xmlWriterSettings, object o)
        {
            FoundationContract.Requires<ArgumentNullException>(xmlSerializer != null);

            var sb = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(sb, xmlWriterSettings))
            {
                xmlSerializer.Serialize(xmlWriter, o);
            }

            return sb.ToString();
        }
    }
}