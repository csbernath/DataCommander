using System;
using System.Web.UI;

namespace DataCommander.Application.ResultWriter;

public class HtmlTextWriterTagRenderer : IDisposable
{
    private readonly HtmlTextWriter _htmlTextWriter;

    public HtmlTextWriterTagRenderer(HtmlTextWriter htmlTextWriter, HtmlTextWriterTag htmlTextWriterTag)
    {
        _htmlTextWriter = htmlTextWriter;
        htmlTextWriter.RenderBeginTag(htmlTextWriterTag);
    }

    public void Dispose() => _htmlTextWriter.RenderEndTag();
}