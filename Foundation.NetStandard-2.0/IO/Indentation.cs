using System;
using System.CodeDom.Compiler;
using Foundation.Diagnostics.Contracts;

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
        public Indentation(IndentedTextWriter textWriter)
        {
            _textWriter = textWriter;
            _indent = ++_textWriter.Indent;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            FoundationContract.Assert(_textWriter.Indent == _indent);

            _textWriter.Indent--;
        }

        #endregion
    }
}