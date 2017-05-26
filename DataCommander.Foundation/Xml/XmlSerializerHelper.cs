using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Foundation.Xml
{
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
#if CONTRACTS_FULL
            Contract.Requires(type != null);
#endif

            var stringReader = new StringReader( xml );
            var xmlSerializer = new XmlSerializer( type );
            var obj = xmlSerializer.Deserialize( stringReader );
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
#if CONTRACTS_FULL
            Contract.Requires(xmlReader != null);
            Contract.Requires(type != null);
#endif

            var xmlSerializer = new XmlSerializer( type );
            var obj = xmlSerializer.Deserialize( xmlReader );
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
            var obj = Deserialize( xml, typeof (T) );
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
            var obj = Deserialize( xmlReader, typeof (T) );
            return (T) obj;
        }
    }
}