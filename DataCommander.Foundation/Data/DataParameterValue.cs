namespace DataCommander.Foundation.Data
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct DataParameterValue<T> : IDataParameterValue<T>
    {
        private static readonly DataParameterValue<T> defaultInstance = new DataParameterValue<T>( DataParameterValueType.Default );
        private static readonly DataParameterValue<T> nullInstance = new DataParameterValue<T>( DataParameterValueType.Null );
        private static readonly DataParameterValue<T> voidInstance = new DataParameterValue<T>( DataParameterValueType.Void );
        private readonly DataParameterValueType type;
        private readonly T value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public DataParameterValue( T value )
        {
            this.type = DataParameterValueType.Value;
            this.value = value;
        }

        private DataParameterValue( DataParameterValueType type )
        {
            this.type = type;
            this.value = default( T );
        }

        /// <summary>
        /// 
        /// </summary>
        public static DataParameterValue<T> Default => defaultInstance;

        /// <summary>
        /// 
        /// </summary>
        public static DataParameterValue<T> Null => nullInstance;

        /// <summary>
        /// 
        /// </summary>
        public static DataParameterValue<T> Void => voidInstance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator DataParameterValue<T>( T value )
        {
            return new DataParameterValue<T>( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator T( DataParameterValue<T> value )
        {
            return value.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataParameterValueType Type => this.type;

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get
            {
                Contract.Assert( this.Type == DataParameterValueType.Value );

                return this.value;
            }
        }

        object IDataParameterValue.ValueObject => this.Value;
    }
}