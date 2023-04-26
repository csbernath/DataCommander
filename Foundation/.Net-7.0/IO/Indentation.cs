using System;
using System.CodeDom.Compiler;
using Foundation.Assertions;

namespace Foundation.IO;

public sealed class Indentation : IDisposable
{
    private readonly IndentedTextWriter _textWriter;
    private readonly int _indent;

    public Indentation(IndentedTextWriter textWriter)
    {
        _textWriter = textWriter;
        _indent = ++_textWriter.Indent;
    }

    #region IDisposable Members

    void IDisposable.Dispose()
    {
        Assert.IsTrue(_textWriter.Indent == _indent);

        --_textWriter.Indent;
    }

    #endregion
}