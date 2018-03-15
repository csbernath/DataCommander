#if FOUNDATION_3_5

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public sealed class Tuple<T1, T2, T3, T4, T5>
    {
        private readonly T1 item1;
        private readonly T2 item2;
        private readonly T3 item3;
        private readonly T4 item4;
        private readonly T5 item5;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="item3"></param>
        /// <param name="item4"></param>
        /// <param name="item5"></param>
        public Tuple( T1 item1, T2 item2, T3 item3, T4 item4, T5 item5 )
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
            this.item5 = item5;
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

        /// <summary>
        /// 
        /// </summary>
        public T3 Item3
        {
            get
            {
                return this.item3;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T4 Item4
        {
            get
            {
                return this.item4;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T5 Item5
        {
            get
            {
                return this.item5;
            }
        }
    }
}

#endif