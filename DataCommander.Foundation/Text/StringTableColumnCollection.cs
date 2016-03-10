namespace DataCommander.Foundation.Text
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public class StringTableColumnCollection : IList<StringTableColumn>
    {
        private readonly List<StringTableColumn> columns = new List<StringTableColumn>();

        #region IList<StringTableColumn> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(StringTableColumn item)
        {
            return this.columns.IndexOf(item);
        }

        void IList<StringTableColumn>.Insert(int index, StringTableColumn item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IList<StringTableColumn>.RemoveAt(int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public StringTableColumn this[int index]
        {
            get
            {
                return this.columns[index];
            }

            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection<StringTableColumn> Members

        internal void Add(StringTableColumn item)
        {
            Contract.Requires(item != null);

            this.columns.Add(item);
        }

        void ICollection<StringTableColumn>.Add(StringTableColumn item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ICollection<StringTableColumn>.Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(StringTableColumn item)
        {
            return this.columns.Contains(item);
        }

        void ICollection<StringTableColumn>.CopyTo(StringTableColumn[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this.columns.Count;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        bool ICollection<StringTableColumn>.Remove(StringTableColumn item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<StringTableColumn> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<StringTableColumn> GetEnumerator()
        {
            return this.columns.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}