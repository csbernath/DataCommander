#if FOUNDATION_3_5

namespace Foundation
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Lazy<T>
    {
        #region Private Fields

        private readonly Func<T> valueFactory;
        private bool isValueCreated;
        private T value;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueFactory"></param>
        public Lazy( Func<T> valueFactory )
        {
            FoundationContract.Requires<ArgumentException>( valueFactory != null );

            this.valueFactory = valueFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsValueCreated
        {
            get
            {
                return this.isValueCreated;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get
            {
                T value;

                if (this.isValueCreated)
                {
                    value = this.value;
                }
                else
                {
                    lock (this)
                    {
                        if (!this.isValueCreated)
                        {
                            this.isValueCreated = true;
                            this.value = this.valueFactory();
                        }

                        value = this.value;
                    }
                }

                return value;
            }
        }
    }
}

#endif