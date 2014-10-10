namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Configuration;
    using System.Xml;

    /// <exclude/>
    public sealed class SectionHandler : IConfigurationSectionHandler
    {
        private readonly XmlNode section;

        private SectionHandler( XmlNode section )
        {
            this.section = section;
        }

        /// <summary>
        /// 
        /// </summary>
        public XmlNode Section
        {
            get
            {
                return this.section;
            }
        }

        Object IConfigurationSectionHandler.Create(
            Object parent,
            Object configContext,
            XmlNode section )
        {
            return new SectionHandler( section );
        }
    }
}