using System.Data;
using System.Xml;

namespace DataCommander.Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDbCommandHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        XmlDocument ExecuteXmlDocument( IDbCommand command );
    }
}