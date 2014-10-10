namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Xml;   

    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlAttributeReader
    {
        private XmlAttributeCollection attributes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        public XmlAttributeReader(XmlAttributeCollection attributes)
        {
            Contract.Requires( attributes != null );

            this.attributes = attributes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean TryGetValue(XmlAttributeCollection attributes, String name, out String value)
        {
            Contract.Requires(attributes != null);

            XmlAttribute attribute = attributes[name];
            Boolean contains = attribute != null;

            if (contains)
            {
                value = attribute.Value;
            }
            else
            {
                value = null;
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetValue(String name, out String value)
        {
            return TryGetValue(this.attributes, name, out value);
        }
    }
}