namespace Foundation.DocumentProperties
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DocumentProperty
    {
        internal DocumentProperty( DocumentPropertyId id, object value )
        {
            Id = id;
            Value = value;
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