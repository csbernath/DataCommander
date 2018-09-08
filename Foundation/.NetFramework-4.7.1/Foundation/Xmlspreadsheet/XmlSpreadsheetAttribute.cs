using System.Xml;
using Foundation.Assertions;

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
            Assert.IsNotNull(xmlWriter);

            xmlWriter.WriteAttributeString(LocalName, Value);
        }
    }
}