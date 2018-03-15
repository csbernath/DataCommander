using System;

namespace Foundation.Collections
{
    /// <exclude/>
    /// <summary>
    /// 
    /// </summary>
    public struct BitVector64
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        [CLSCompliant(false)]
        public BitVector64(ulong data)
        {
            Value = data;
        }

        /// <summary>
        /// 
        /// </summary>
        [CLSCompliant(false)]
        public ulong Value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool this[int index]
        {
            get
            {
                var bit = 1UL << index;
                return (Value & bit) == bit;
            }

            set
            {
                var bit = 1UL << index;

                if (value)
                {
                    Value |= bit;
                }
                else
                {
                    Value &= ~bit;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var value = Value.ToString("X");
            value = value.PadLeft(16, '0');
            return value;
        }
    }
}