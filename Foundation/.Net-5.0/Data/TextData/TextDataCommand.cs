using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;

namespace Foundation.Data.TextData
{
    public sealed class TextDataCommand : DbCommand
    {
        private TextDataConnection _connection;

        #region Constructors

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public new TextDataParameterCollection Parameters { get; } = new();

        /// <summary>
        /// 
        /// </summary>
        public override string CommandText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override int CommandTimeout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override CommandType CommandType { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new TextDataReader ExecuteReader()
        {
            return new TextDataReader(this, CommandBehavior.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public new TextDataReader ExecuteReader(CommandBehavior behavior)
        {
            return new TextDataReader(this, behavior);
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
            get => _connection;

            set => _connection = (TextDataConnection)value;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override DbParameterCollection DbParameterCollection => Parameters;

        /// <summary>
        /// 
        /// </summary>
        protected override DbTransaction DbTransaction
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool DesignTimeVisible
        {
            get => false;

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return new TextDataReader(this, behavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int ExecuteNonQuery()
        {
            var columns = Parameters.GetParameterValue<IList<TextDataColumn>>("columns");
            var converters = Parameters.GetParameterValue<IList<ITextDataConverter>>("converters");
            var rows = Parameters.GetParameterValue<IEnumerable<object[]>>("rows");
            var getTextWriter = Parameters.GetParameterValue<IConverter<TextDataCommand, TextWriter>>("getTextWriter");
            var textWriter = getTextWriter.Convert(this);
            var textDataStreamWriter = new TextDataStreamWriter(textWriter, columns, converters);
            var count = 0;

            foreach (var row in rows)
            {
                textDataStreamWriter.WriteRow(row);
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
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        #endregion
    }
}