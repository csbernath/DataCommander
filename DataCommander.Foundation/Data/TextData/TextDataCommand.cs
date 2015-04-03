namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public interface IConverter<in TInput, out TOutput>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        TOutput Convert( TInput input );
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataCommand : DbCommand
    {
        private TextDataConnection connection;
        private CommandType commandType;
        private int commandTimeout;
        private string commandText;
        private readonly TextDataParameterCollection parameters = new TextDataParameterCollection();

        #region Constructors
        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public new TextDataParameterCollection Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string CommandText
        {
            get
            {
                return this.commandText;
            }

            set
            {
                this.commandText = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int CommandTimeout
        {
            get
            {
                return this.commandTimeout;
            }

            set
            {
                this.commandTimeout = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override CommandType CommandType
        {
            get
            {
                return this.commandType;
            }

            set
            {
                this.commandType = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new TextDataReader ExecuteReader()
        {
            return new TextDataReader( this, CommandBehavior.Default );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public new TextDataReader ExecuteReader( CommandBehavior behavior )
        {
            return new TextDataReader( this, behavior );
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DbParameter CreateDbParameter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override DbConnection DbConnection
        {
            get
            {
                return this.connection;
            }

            set
            {
                this.connection = (TextDataConnection) value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override DbParameterCollection DbParameterCollection
        {
            get
            {
                return this.parameters;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override DbTransaction DbTransaction
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool DesignTimeVisible
        {
            get
            {
                return false;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        protected override DbDataReader ExecuteDbDataReader( CommandBehavior behavior )
        {
            return new TextDataReader( this, behavior );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int ExecuteNonQuery()
        {
            IList<TextDataColumn> columns = this.parameters.GetParameterValue<IList<TextDataColumn>>( "columns" );
            IList<ITextDataConverter> converters = this.parameters.GetParameterValue<IList<ITextDataConverter>>( "converters" );
            IEnumerable<object[]> rows = this.parameters.GetParameterValue<IEnumerable<object[]>>( "rows" );
            IConverter<TextDataCommand, TextWriter> getTextWriter = this.parameters.GetParameterValue<IConverter<TextDataCommand, TextWriter>>( "getTextWriter" );
            TextWriter textWriter = getTextWriter.Convert( this );
            TextDataStreamWriter textDataStreamWriter = new TextDataStreamWriter( textWriter, columns, converters );
            int count = 0;

            foreach (object[] row in rows)
            {
                textDataStreamWriter.WriteRow( row );
                count++;
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Prepare()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}