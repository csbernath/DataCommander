using System.Text;
using System.Xml;
using System.Xml.Serialization;

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
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(xmlSerializer != null);
#endif

            var sb = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(sb, xmlWriterSettings))
            {
                xmlSerializer.Serialize(xmlWriter, o);
            }

            return sb.ToString();
        }
    }
}