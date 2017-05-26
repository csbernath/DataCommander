using System;
using System.Xml;

namespace Foundation.Xml
{
    /// <summary>
    /// 
    /// </summary>
    public static class XmlWriterExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="localName"></param>
        /// <returns></returns>
        public static IDisposable WriteElement(this XmlWriter xmlWriter, string localName)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(xmlWriter != null);
#endif

            xmlWriter.WriteStartElement(localName);
            return new Disposer(xmlWriter.WriteEndElement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="prefix"></param>
        /// <param name="localName"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        public static IDisposable WriteElement(this XmlWriter xmlWriter, string prefix, string localName, string ns)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(xmlWriter != null);
#endif

            xmlWriter.WriteStartElement(prefix, localName, ns);
            return new Disposer(xmlWriter.WriteEndElement);
        }
    }
}