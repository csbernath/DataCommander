namespace DataCommander.Foundation.XmlSpreadsheet
{
    using System.Xml;
    using DataCommander.Foundation.Xml;

    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlSpreadsheetCell
    {
        private readonly XmlSpreadsheetAttributeCollection _attributes = new XmlSpreadsheetAttributeCollection();
        private readonly XmlSpreadsheetDataType _dataType;
        private readonly string _value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="value"></param>
        public XmlSpreadsheetCell(XmlSpreadsheetDataType dataType, string value)
        {
            this._dataType = dataType;
            this._value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string StyleId
        {
            set
            {
                var attribute = new XmlSpreadsheetAttribute("ss:StyleID", value);
                this._attributes.Add(attribute);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MergeAcross
        {
            set
            {
                var attribute = new XmlSpreadsheetAttribute("ss:MergeAcross", value.ToString());
                this._attributes.Add(attribute);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        public void Write(XmlWriter xmlWriter)
        {
#if CONTRACTS_FULL
            Contract.Requires(xmlWriter != null);
#endif

            using (xmlWriter.WriteElement("Cell"))
            {
                foreach (var attribute in this._attributes)
                {
                    attribute.Write(xmlWriter);
                }

                using (xmlWriter.WriteElement("Data"))
                {
                    xmlWriter.WriteAttributeString("ss:Type", this._dataType.ToString());
                    xmlWriter.WriteString(this._value);
                }
            }
        }
    }
}