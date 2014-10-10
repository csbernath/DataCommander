namespace DataCommander.Foundation.XmlSpreadsheet
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Xml;
    using DataCommander.Foundation.Xml;

    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlSpreadsheetCell
    {
        private readonly XmlSpreadsheetAttributeCollection attributes = new XmlSpreadsheetAttributeCollection();
        private XmlSpreadsheetDataType dataType;
        private String value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="value"></param>
        public XmlSpreadsheetCell( XmlSpreadsheetDataType dataType, String value )
        {
            this.dataType = dataType;
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public String StyleId
        {
            set
            {
                var attribute = new XmlSpreadsheetAttribute( "ss:StyleID", value );
                this.attributes.Add( attribute );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 MergeAcross
        {
            set
            {
                var attribute = new XmlSpreadsheetAttribute( "ss:MergeAcross", value.ToString() );
                this.attributes.Add( attribute );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        public void Write( XmlWriter xmlWriter )
        {
            Contract.Requires( xmlWriter != null );

            using (xmlWriter.WriteElement( "Cell" ))
            {
                foreach (var attribute in this.attributes)
                {
                    attribute.Write( xmlWriter );
                }

                using (xmlWriter.WriteElement( "Data" ))
                {
                    xmlWriter.WriteAttributeString( "ss:Type", this.dataType.ToString() );
                    xmlWriter.WriteString( this.value );
                }
            }
        }
    }
}