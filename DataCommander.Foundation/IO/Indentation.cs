namespace DataCommander.Foundation.IO
{
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class Indentation : IDisposable
    {
        private readonly IndentedTextWriter textWriter;
        private readonly Int32 indent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        public Indentation( IndentedTextWriter textWriter )
        {
            this.textWriter = textWriter;
            this.indent = ++this.textWriter.Indent;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Contract.Assert( this.textWriter.Indent == this.indent );

            this.textWriter.Indent--;
        }

        #endregion
    }
}