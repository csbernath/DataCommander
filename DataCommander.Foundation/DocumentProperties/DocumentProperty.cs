namespace Foundation.DocumentProperties
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DocumentProperty
    {
        internal DocumentProperty( DocumentPropertyId id, object value )
        {
            this.Id = id;
            this.Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public DocumentPropertyId Id { get; }

        /// <summary>
        /// 
        /// </summary>
        public object Value { get; }
    }
}