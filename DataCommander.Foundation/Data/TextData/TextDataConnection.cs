namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Data.Common;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataConnection : DbConnection
    {
        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new TextDataCommand CreateCommand()
        {
            var command = new TextDataCommand { Connection = this };
            return command;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        protected override DbTransaction BeginDbTransaction( IsolationLevel isolationLevel )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        public override void ChangeDatabase( String databaseName )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override String ConnectionString
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
        /// <returns></returns>
        protected override DbCommand CreateDbCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override String DataSource
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override String Database
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Open()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override String ServerVersion
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override ConnectionState State
        {
            get
            {
                return ConnectionState.Open;
            }
        }
    }
}