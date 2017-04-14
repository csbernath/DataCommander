namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Collections;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataParameterCollection : DbParameterCollection, IList<TextDataParameter>
    {
        private readonly IndexableCollection<TextDataParameter> collection;
        private readonly ListIndex<TextDataParameter> listIndex;
        private readonly UniqueIndex<string, TextDataParameter> nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public TextDataParameterCollection()
        {
            this.listIndex = new ListIndex<TextDataParameter>("List");
            this.nameIndex = new UniqueIndex<string, TextDataParameter>(
                "Name",
                parameter => GetKeyResponse.Create(true, parameter.ParameterName),
                SortOrder.None);

            this.collection = new IndexableCollection<TextDataParameter>(this.listIndex);
            this.collection.Indexes.Add(this.nameIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override int Add(object value)
        {
#if CONTRACTS_FULL
            Contract.Requires( value != null );
            Contract.Requires( value is TextDataParameter );
#endif

            var parameter = (TextDataParameter)value;
            this.collection.Add(parameter);
            return this.collection.Count - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public TextDataParameter Add(TextDataParameter parameter)
        {
#if CONTRACTS_FULL
            Contract.Assert(parameter != null);
#endif

            this.collection.Add(parameter);
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
            this.collection.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public override bool Contains(string value)
        {
            return this.nameIndex.ContainsKey(value);
        }

#if FOUNDATION_3_5
#else
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public bool PureContains(string value)
        {
            return this.Contains(value);
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
#if FOUNDATION_3_5
#else
        [Pure]
#endif
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
        public override int Count => this.collection.Count;

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
#if CONTRACTS_FULL
            Contract.Assert(this.Contains(parameterName));
#endif
            var parameter = this.nameIndex[parameterName];
            var value = parameter.Value;
#if CONTRACTS_FULL
            Contract.Assert(value is TResult);
#endif
            return (TResult)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool TryGetValue(string parameterName, out TextDataParameter parameter)
        {
            return this.nameIndex.TryGetValue(parameterName, out parameter);
        }

#region IList<TextDataParameter> Members

        int IList<TextDataParameter>.IndexOf(TextDataParameter item)
        {
#if CONTRACTS_FULL
            Contract.Assert(item != null);
#endif

            return this.listIndex.IndexOf(item);
        }

        void IList<TextDataParameter>.Insert(int index, TextDataParameter item)
        {
            throw new NotImplementedException();
        }

        void IList<TextDataParameter>.RemoveAt(int index)
        {
            var parameter = this.listIndex[index];
            this.collection.Remove(parameter);
        }

        TextDataParameter IList<TextDataParameter>.this[int index]
        {
            get => this.listIndex[index];

            set => throw new NotSupportedException();
        }

#endregion

#region ICollection<TextDataParameter> Members

        void ICollection<TextDataParameter>.Add(TextDataParameter item)
        {
#if CONTRACTS_FULL
            Contract.Assert(item != null);
#endif
            this.collection.Add(item);
        }

        void ICollection<TextDataParameter>.Clear()
        {
            this.collection.Clear();
        }

        bool ICollection<TextDataParameter>.Contains(TextDataParameter item)
        {
            throw new NotImplementedException();
        }

        void ICollection<TextDataParameter>.CopyTo(TextDataParameter[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<TextDataParameter>.Count => this.collection.Count;

        bool ICollection<TextDataParameter>.IsReadOnly => this.collection.IsReadOnly;

        bool ICollection<TextDataParameter>.Remove(TextDataParameter item)
        {
#if CONTRACTS_FULL
            Contract.Assert(item != null);
#endif
            return this.collection.Remove(item);
        }

#endregion

#region IEnumerable<TextDataParameter> Members

        IEnumerator<TextDataParameter> IEnumerable<TextDataParameter>.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

#endregion

#region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

#endregion
    }
}