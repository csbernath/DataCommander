using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Contracts;
using Foundation.Assertions;
using Foundation.Collections.IndexableCollection;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data.TextData
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataParameterCollection : DbParameterCollection, IList<TextDataParameter>
    {
        private readonly IndexableCollection<TextDataParameter> _collection;
        private readonly ListIndex<TextDataParameter> _listIndex;
        private readonly UniqueIndex<string, TextDataParameter> _nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public TextDataParameterCollection()
        {
            _listIndex = new ListIndex<TextDataParameter>("List");
            _nameIndex = new UniqueIndex<string, TextDataParameter>(
                "Name",
                parameter => GetKeyResponse.Create(true, parameter.ParameterName),
                SortOrder.None);

            _collection = new IndexableCollection<TextDataParameter>(_listIndex);
            _collection.Indexes.Add(_nameIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override int Add(object value)
        {
            Assert.IsNotNull(value);
            FoundationContract.Requires<ArgumentException>(value is TextDataParameter);

            var parameter = (TextDataParameter) value;
            _collection.Add(parameter);
            return _collection.Count - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public TextDataParameter Add(TextDataParameter parameter)
        {
            Assert.IsTrue(parameter != null);

            _collection.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public override void AddRange(Array values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            _collection.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public override bool Contains(string value)
        {
            return _nameIndex.ContainsKey(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public override void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override int Count => _collection.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        protected override DbParameter GetParameter(string parameterName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override DbParameter GetParameter(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public override int IndexOf(string parameterName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public override void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsFixedSize => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public override bool IsReadOnly => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public override bool IsSynchronized => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void Remove(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        public override void RemoveAt(string parameterName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void SetParameter(int index, DbParameter value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override object SyncRoot => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public TResult GetParameterValue<TResult>(string parameterName)
        {
            Assert.IsTrue(Contains(parameterName));

            var parameter = _nameIndex[parameterName];
            var value = parameter.Value;

            Assert.IsTrue(value is TResult);
            return (TResult) value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool TryGetValue(string parameterName, out TextDataParameter parameter)
        {
            return _nameIndex.TryGetValue(parameterName, out parameter);
        }

        #region IList<TextDataParameter> Members

        int IList<TextDataParameter>.IndexOf(TextDataParameter item)
        {
            Assert.IsTrue(item != null);

            return _listIndex.IndexOf(item);
        }

        void IList<TextDataParameter>.Insert(int index, TextDataParameter item)
        {
            throw new NotImplementedException();
        }

        void IList<TextDataParameter>.RemoveAt(int index)
        {
            var parameter = _listIndex[index];
            _collection.Remove(parameter);
        }

        TextDataParameter IList<TextDataParameter>.this[int index]
        {
            get => _listIndex[index];

            set => throw new NotSupportedException();
        }

        #endregion

        #region ICollection<TextDataParameter> Members

        void ICollection<TextDataParameter>.Add(TextDataParameter item)
        {
            Assert.IsTrue(item != null);

            _collection.Add(item);
        }

        void ICollection<TextDataParameter>.Clear()
        {
            _collection.Clear();
        }

        bool ICollection<TextDataParameter>.Contains(TextDataParameter item)
        {
            throw new NotImplementedException();
        }

        void ICollection<TextDataParameter>.CopyTo(TextDataParameter[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<TextDataParameter>.Count => _collection.Count;

        bool ICollection<TextDataParameter>.IsReadOnly => _collection.IsReadOnly;

        bool ICollection<TextDataParameter>.Remove(TextDataParameter item)
        {
            Assert.IsTrue(item != null);

            return _collection.Remove(item);
        }

        #endregion

        #region IEnumerable<TextDataParameter> Members

        IEnumerator<TextDataParameter> IEnumerable<TextDataParameter>.GetEnumerator()
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
    }
}