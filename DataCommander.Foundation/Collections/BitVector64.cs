namespace DataCommander.Foundation.Collections
{
    using System;

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
        public BitVector64(UInt64 data)
        {
            this.Value = data;
        }

        /// <summary>
        /// 
        /// </summary>
        [CLSCompliant(false)]
        public UInt64 Value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool this[int index]
        {
            get
            {
                var bit = 1UL << index;
                return (this.Value & bit) == bit;
            }

            set
            {
                var bit = 1UL << index;

                if (value)
                {
                    this.Value |= bit;
                }
                else
                {
                    this.Value &= ~bit;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var value = this.Value.ToString("X");
            value = value.PadLeft(16, '0');
            return value;
        }
    }
}