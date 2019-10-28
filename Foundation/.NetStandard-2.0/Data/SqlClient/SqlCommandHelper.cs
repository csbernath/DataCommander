using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace Foundation.Data.SqlClient
{
    public class SqlCommandHelper : IDbCommandHelper
    {
        public XmlDocument ExecuteXmlDocument(IDbCommand command)
        {
            var sqlCommand = (SqlCommand) command;
            var xmlDocument = new XmlDocument();
            XmlReader xmlReader = null;

            try
            {
                xmlReader = sqlCommand.ExecuteXmlReader();
                xmlDocument.Load(xmlReader);
            }
            finally
            {
                if (xmlReader != null)
                    xmlReader.Close();
            }

            return xmlDocument;
        }
    }
}