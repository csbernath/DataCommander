using System;
using System.Xml;
using Foundation.Assertions;
using Foundation.Core;

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
            Assert.IsNotNull(xmlWriter);

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
            Assert.IsNotNull(xmlWriter);

            xmlWriter.WriteStartElement(prefix, localName, ns);
            return new Disposer(xmlWriter.WriteEndElement);
        }
    }
}