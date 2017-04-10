namespace DataCommander.Foundation.IO
{
    using System;
    using System.CodeDom.Compiler;

    /// <summary>
    /// 
    /// </summary>
    public sealed class Indentation : IDisposable
    {
        private readonly IndentedTextWriter textWriter;
        private readonly int indent;

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
#if CONTRACTS_FULL
            Contract.Assert( this.textWriter.Indent == this.indent );
#endif

            this.textWriter.Indent--;
        }

#endregion
    }
}