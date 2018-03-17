using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using Foundation.Collections.IndexableCollection;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data.TextData
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataColumnCollection : IList<TextDataColumn>
    {
        private readonly IndexableCollection<TextDataColumn> _collection;
        private readonly ListIndex<TextDataColumn> _listIndex;
        private readonly UniqueIndex<string, TextDataColumn> _nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumnCollection()
        {
            _listIndex = new ListIndex<TextDataColumn>("List");

            _nameIndex = new UniqueIndex<string, TextDataColumn>(
                "Name",
                column => GetKeyResponse.Create(true, column.ColumnName),
                SortOrder.None);

            _collection = new IndexableCollection<TextDataColumn>(_listIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public TextDataColumn this[string columnName]
        {
            get
            {
                Assert.IsValidOperation(Contains(columnName));

                return _nameIndex[columnName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        [Pure]
        public bool Contains(string columnName)
        {
            return _nameIndex.ContainsKey(columnName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int IndexOf(string columnName)
        {
            TextDataColumn column;
            var contains = _nameIndex.TryGetValue(columnName, out column);
            int index;

            if (contains)
            {
                index = _listIndex.IndexOf(column);
            }
            else
            {
                index = -1;
            }

            return index;
        }

        internal int IndexOf(string columnName, bool throwException)
        {
            var index = IndexOf(columnName);

            if (index < 0)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Column '{0} not found.", columnName);
                throw new IndexOutOfRangeException(message);
            }

            return index;
        }

        internal int IndexOf(TextDataColumn column, bool throwException)
        {
            var index = IndexOf(column);

            if (index < 0)
            {
                throw new ArgumentException("Column is not in ColumnList");
            }

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TextDataColumn this[int index]
        {
            get
            {
                FoundationContract.Assert(index >= 0);
                FoundationContract.Assert(index < _collection.Count);

                return _listIndex[index];
            }

            set { throw new NotImplementedException(); }
        }

        #region ICollection<TextDataColumn> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(TextDataColumn item)
        {
            _collection.Add(item);
        }

        void ICollection<TextDataColumn>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<TextDataColumn>.Contains(TextDataColumn item)
        {
            throw new NotImplementedException();
        }

        void ICollection<TextDataColumn>.CopyTo(TextDataColumn[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => _collection.Count;

        bool ICollection<TextDataColumn>.IsReadOnly => throw new NotImplementedException();

        bool ICollection<TextDataColumn>.Remove(TextDataColumn item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<TextDataColumn> Members

        IEnumerator<TextDataColumn> IEnumerable<TextDataColumn>.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region IList<TextDataColumn> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(TextDataColumn item)
        {
            return _listIndex.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, TextDataColumn item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}