namespace DataCommander.Foundation.XmlSpreadsheet
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Xml;

    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlSpreadsheetAttribute
    {
        private readonly string localName;
        private readonly string value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="value"></param>
        public XmlSpreadsheetAttribute(string localName, string value)
        {
            this.localName = localName;
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string LocalName
        {
            get
            {
                return this.localName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        public void Write(XmlWriter xmlWriter)
        {
            Contract.Requires<ArgumentNullException>(xmlWriter != null);

            xmlWriter.WriteAttributeString(this.localName, this.value);
        }
    }
}