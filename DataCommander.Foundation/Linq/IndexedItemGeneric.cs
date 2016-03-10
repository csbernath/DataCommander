namespace DataCommander.Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct IndexedItem<T>
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
        public int Index => this.index;

        /// <summary>
        /// 
        /// </summary>
        public T Value => this.value;
    }
}