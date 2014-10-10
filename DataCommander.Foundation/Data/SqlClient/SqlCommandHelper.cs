namespace DataCommander.Foundation.Data.SqlClient
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Xml;

    /// <summary>
    /// 
    /// </summary>
    public class SqlCommandHelper : IDbCommandHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public XmlDocument ExecuteXmlDocument( IDbCommand command )
        {
            var sqlCommand = (SqlCommand) command;
            var xmlDocument = new XmlDocument();
            XmlReader xmlReader = null;

            try
            {
                xmlReader = sqlCommand.ExecuteXmlReader();
                xmlDocument.Load( xmlReader );
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }

            return xmlDocument;
        }
    }
}