namespace DataCommander.Foundation.Xml
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// 
    /// </summary>
    public static class XmlSerializerHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialize( string xml, Type type )
        {
            Contract.Requires(type != null);

            var stringReader = new StringReader( xml );
            var xmlSerializer = new XmlSerializer( type );
            object obj = xmlSerializer.Deserialize( stringReader );
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialize( XmlReader xmlReader, Type type )
        {
            Contract.Requires(xmlReader != null);
            Contract.Requires(type != null);

            var xmlSerializer = new XmlSerializer( type );
            object obj = xmlSerializer.Deserialize( xmlReader );
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>( string xml )
        {
            object obj = Deserialize( xml, typeof (T) );
            return (T) obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlReader"></param>
        /// <returns></returns>
        public static T Deserialize<T>( XmlReader xmlReader )
        {
            object obj = Deserialize( xmlReader, typeof (T) );
            return (T) obj;
        }
    }
}