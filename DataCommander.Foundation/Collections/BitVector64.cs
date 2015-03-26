namespace DataCommander.Foundation.Collections
{
    using System;

    /// <exclude/>
    /// <summary>
    /// 
    /// </summary>
    public struct BitVector64
    {
        private UInt64 data;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        [CLSCompliant(false)]
        public BitVector64(UInt64 data)
        {
            this.data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        [CLSCompliant(false)]
        public UInt64 Value
        {
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool this[int index]
        {
            get
            {
                UInt64 bit = 1UL << index;
                return (this.data & bit) == bit;
            }

            set
            {
                UInt64 bit = 1UL << index;

                if (value)
                {
                    this.data |= bit;
                }
                else
                {
                    this.data &= ~bit;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string value = this.data.ToString("X");
            value = value.PadLeft(16, '0');
            return value;
        }
    }
}