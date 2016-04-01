namespace DataCommander.Foundation.Configuration
{
    using System.Configuration;
    using System.Xml;

    /// <exclude/>
    public sealed class SectionHandler : IConfigurationSectionHandler
    {
        private SectionHandler(XmlNode section)
        {
            this.Section = section;
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