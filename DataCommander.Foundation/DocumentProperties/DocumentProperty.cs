namespace DataCommander.Foundation.DocumentProperties
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class DocumentProperty
    {
        private readonly DocumentPropertyId id;
        private readonly Object value;

        internal DocumentProperty( DocumentPropertyId id, Object value )
        {
            this.id = id;
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public DocumentPropertyId Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Object Value
        {
            get
            {
                return this.value;
            }
        }
    }
}