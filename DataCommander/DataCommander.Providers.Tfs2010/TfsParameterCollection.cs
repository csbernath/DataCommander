namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    internal sealed class TfsParameterCollection : IDataParameterCollection, IEnumerable<TfsParameter>
    {
        private readonly List<TfsParameter> list = new List<TfsParameter>();

        public TfsParameter this[int index] => this.list[index];

        public TfsParameter this[string parameterName] => this.GetParameter(parameterName);

        public void Add(TfsParameter parameter)
        {
            this.list.Add(parameter);
        }

        public void AddBooleanInput(string name, bool isNullable, bool defaultValue)
        {
            var parameter = new TfsParameter(name, typeof (bool), DbType.Boolean, ParameterDirection.Input, isNullable,
                defaultValue);
            this.Add(parameter);
        }

        public void AddInt32Input(string name, bool isNullable, int defaultValue)
        {
            var parameter = new TfsParameter(name, typeof (int), DbType.Int32, ParameterDirection.Input, isNullable,
                defaultValue);
            this.Add(parameter);
        }

        public void AddStringInput(string name, bool isNullable, object defaultValue)
        {
            var parameter = new TfsParameter(name, typeof (string), DbType.String, ParameterDirection.Input, isNullable,
                defaultValue);
            this.Add(parameter);
        }

        public void AddValueTypeInput<T>(string name, T defaultValue) where T : struct
        {
            var parameter = new TfsParameter(name, typeof (T), DbType.Object, ParameterDirection.Input, true,
                defaultValue);
            this.Add(parameter);
        }

        #region IDataParameterCollection Members

        bool IDataParameterCollection.Contains(string parameterName)
        {
            throw new NotImplementedException();
        }

        int IDataParameterCollection.IndexOf(string parameterName)
        {
            throw new NotImplementedException();
        }

        void IDataParameterCollection.RemoveAt(string parameterName)
        {
            throw new NotImplementedException();
        }

        private TfsParameter GetParameter(string parameterName)
        {
            var parameter = this.list.First(p => p.ParameterName == parameterName);
            return parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        object IDataParameterCollection.this[string parameterName]
        {
            get => this.GetParameter(parameterName);

            set => throw new NotImplementedException();
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Clear()
        {
            this.list.Clear();
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        int IList.IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        bool IList.IsFixedSize => throw new NotImplementedException();

        bool IList.IsReadOnly => throw new NotImplementedException();

        void IList.Remove(object value)
        {
            var parameter = (TfsParameter) value;
            this.list.Remove(parameter);
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get => this.list[index];

            set
            {
                var parameter = (TfsParameter) value;
                this.list[index] = parameter;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count => this.list.Count;

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion

        #region IEnumerable<TfsParameter> Members

        IEnumerator<TfsParameter> IEnumerable<TfsParameter>.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion
    }
}