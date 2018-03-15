#if FOUNDATION_3_5

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public sealed class Tuple<T1, T2>
    {
        private readonly T1 item1;
        private readonly T2 item2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        public Tuple( T1 item1, T2 item2 )
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        /// <summary>
        /// 
        /// </summary>
        public T1 Item1
        {
            get
            {
                return this.item1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T2 Item2
        {
            get
            {
                return this.item2;
            }
        }
    }
}

#endif