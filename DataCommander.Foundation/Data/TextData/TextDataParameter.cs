namespace DataCommander.Foundation.Data.TextData
{
    using System;
    using System.Data;
    using System.Data.Common;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataParameter : DbParameter
    {
        private string _name;
        private object _value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public TextDataParameter(string name, object value)
        {
            this._name = name;
            this._value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override DbType DbType
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override ParameterDirection Direction
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsNullable
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ParameterName
        {
            get => this._name;

            set => this._name = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ResetDbType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override int Size
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override string SourceColumn
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool SourceColumnNullMapping
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override DataRowVersion SourceVersion
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override object Value
        {
            get => this._value;

            set => this._value = value;
        }
    }
}