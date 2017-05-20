namespace DataCommander.Foundation.Configuration
{
    using System.Xml;

    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlAttributeReader
    {
        private readonly XmlAttributeCollection _attributes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        public XmlAttributeReader(XmlAttributeCollection attributes)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(attributes != null);
#endif
            this._attributes = attributes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValue(XmlAttributeCollection attributes, string name, out string value)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(attributes != null);
#endif

            var attribute = attributes[name];
            var contains = attribute != null;

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
        public bool TryGetValue(string name, out string value)
        {
            return TryGetValue(this._attributes, name, out value);
        }
    }
}