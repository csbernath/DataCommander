using System.Data;
using System.Xml;

namespace Foundation.Data
{
    public interface IDbCommandHelper
    {
        XmlDocument ExecuteXmlDocument(IDbCommand command);
    }
}