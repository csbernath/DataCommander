namespace DataCommander.Foundation.Data.PTypes
{
    using System.Data.SqlTypes;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public struct PXml
    {
        private readonly SqlXml sqlXml;
        private PValueType type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PXml Null = new PXml( PValueType.Null );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PXml Default = new PXml( PValueType.Default );

        /// <summary>
        /// 
        /// </summary>
        public static readonly PXml Empty = new PXml( PValueType.Empty );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        private PXml( PValueType type )
        {
            this.type = type;
            this.sqlXml = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PXml( SqlXml value )
        {
            this.sqlXml = value;

            if (value != null)
            {
                this.type = PValueType.Value;
            }
            else
            {
                this.type = PValueType.Null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value => this.sqlXml;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static implicit operator PXml( SqlXml value )
        {
            return new PXml( value );
        }
    }
}