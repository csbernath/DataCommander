namespace DataCommander.Foundation.DocumentProperties
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DocumentProperty
    {
        private readonly DocumentPropertyId id;
        private readonly object value;

        internal DocumentProperty( DocumentPropertyId id, object value )
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
        public object Value
        {
            get
            {
                return this.value;
            }
        }
    }
}