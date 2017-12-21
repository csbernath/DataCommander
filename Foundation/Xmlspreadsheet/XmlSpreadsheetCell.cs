using System;
using System.Xml;
using Foundation.Diagnostics.Contracts;
using Foundation.Xml;

namespace Foundation.XmlSpreadsheet
{
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
            _dataType = dataType;
            _value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string StyleId
        {
            set
            {
                var attribute = new XmlSpreadsheetAttribute("ss:StyleID", value);
                _attributes.Add(attribute);
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
                _attributes.Add(attribute);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        public void Write(XmlWriter xmlWriter)
        {
            FoundationContract.Requires<ArgumentException>(xmlWriter != null);

            using (xmlWriter.WriteElement("Cell"))
            {
                foreach (var attribute in _attributes)
                {
                    attribute.Write(xmlWriter);
                }

                using (xmlWriter.WriteElement("Data"))
                {
                    xmlWriter.WriteAttributeString("ss:Type", _dataType.ToString());
                    xmlWriter.WriteString(_value);
                }
            }
        }
    }
}