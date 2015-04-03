namespace DataCommander.Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class IndexedItem<T>
    {
        #region Private Fields

        private readonly int index;
        private readonly T value;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public IndexedItem(int index, T value)
        {
            this.index = index;
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Index
        {
            get
            {
                return this.index;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
            }
        }
    }
}