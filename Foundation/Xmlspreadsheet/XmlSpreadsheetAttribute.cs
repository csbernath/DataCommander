﻿using System.Xml;

namespace Foundation.XmlSpreadsheet
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlSpreadsheetAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="value"></param>
        public XmlSpreadsheetAttribute(string localName, string value)
        {
            LocalName = localName;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string LocalName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        public void Write(XmlWriter xmlWriter)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(xmlWriter != null);
#endif

            xmlWriter.WriteAttributeString(LocalName, Value);
        }
    }
}