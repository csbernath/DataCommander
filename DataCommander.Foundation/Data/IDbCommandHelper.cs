namespace DataCommander.Foundation.Data
{
    using System.Data;
    using System.Xml;

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