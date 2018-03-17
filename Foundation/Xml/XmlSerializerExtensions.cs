using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;

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
            Assert.IsNotNull(xmlSerializer);

            var sb = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(sb, xmlWriterSettings))
            {
                xmlSerializer.Serialize(xmlWriter, o);
            }

            return sb.ToString();
        }
    }
}