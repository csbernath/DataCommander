using System.Configuration;
using System.Xml;

namespace Foundation.Configuration
{
    /// <exclude/>
    public sealed class SectionHandler : IConfigurationSectionHandler
    {
        private SectionHandler(XmlNode section)
        {
            Section = section;
        }

        /// <summary>
        /// 
        /// </summary>
        public XmlNode Section { get; }

        object IConfigurationSectionHandler.Create(
            object parent,
            object configContext,
            XmlNode section)
        {
            return new SectionHandler(section);
        }
    }
}