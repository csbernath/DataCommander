namespace DataCommander.Foundation.Xml
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Xml;

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
        public static IDisposable WriteElement( this XmlWriter xmlWriter, string localName )
        {
            Contract.Requires( xmlWriter != null );

            xmlWriter.WriteStartElement( localName );
            return new Disposer( xmlWriter.WriteEndElement );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="prefix"></param>
        /// <param name="localName"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        public static IDisposable WriteElement( this XmlWriter xmlWriter, string prefix, string localName, string ns )
        {
            Contract.Requires( xmlWriter != null );

            xmlWriter.WriteStartElement( prefix, localName, ns );
            return new Disposer( xmlWriter.WriteEndElement );
        }
    }
}