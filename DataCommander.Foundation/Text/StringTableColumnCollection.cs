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
        private List<StringTableColumn> columns = new List<StringTableColumn>();

        #region IList<StringTableColumn> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Int32 IndexOf(StringTableColumn item)
        {
            return this.columns.IndexOf(item);
        }

        void IList<StringTableColumn>.Insert(Int32 index, StringTableColumn item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IList<StringTableColumn>.RemoveAt(Int32 index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public StringTableColumn this[Int32 index]
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
        public Boolean Contains(StringTableColumn item)
        {
            return this.columns.Contains(item);
        }

        void ICollection<StringTableColumn>.CopyTo(StringTableColumn[] array, Int32 arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.columns.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsReadOnly
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        Boolean ICollection<StringTableColumn>.Remove(StringTableColumn item)
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