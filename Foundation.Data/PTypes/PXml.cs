using System.Data.SqlTypes;
using System.Diagnostics;

namespace Foundation.Data.PTypes
{
    /// <summary>
    /// 
    /// </summary>
    public struct PXml
    {
        private readonly SqlXml _sqlXml;
        private PValueType _type;

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
            _type = type;
            _sqlXml = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PXml( SqlXml value )
        {
            _sqlXml = value;

            _type = value != null ? PValueType.Value : PValueType.Null;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value => _sqlXml;

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