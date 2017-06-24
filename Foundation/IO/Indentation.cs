using System;
using System.CodeDom.Compiler;

namespace Foundation.IO
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Indentation : IDisposable
    {
        private readonly IndentedTextWriter _textWriter;
        private readonly int _indent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        public Indentation( IndentedTextWriter textWriter )
        {
            _textWriter = textWriter;
            _indent = ++_textWriter.Indent;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
#if CONTRACTS_FULL
            Contract.Assert( this.textWriter.Indent == this.indent );
#endif

            _textWriter.Indent--;
        }

#endregion
    }
}