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
        private String localName;
        private readonly String value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="value"></param>
        public XmlSpreadsheetAttribute( String localName, String value )
        {
            this.localName = localName;
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public String LocalName
        {
            get
            {
                return this.localName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Value
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
        public void Write( XmlWriter xmlWriter )
        {
            Contract.Requires( xmlWriter != null );

            xmlWriter.WriteAttributeString( this.localName, this.value );
        }
    }
}