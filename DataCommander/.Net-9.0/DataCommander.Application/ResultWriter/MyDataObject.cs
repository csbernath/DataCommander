using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Application.ResultWriter;

internal sealed class MyDataObject(DataView dataView, int[] columnIndexes) : IDataObject
{
    private readonly int[] _columnIndexes = columnIndexes;

    object IDataObject.GetData(Type format) => throw new NotImplementedException();

    object IDataObject.GetData(string format) => throw new NotImplementedException();

    object IDataObject.GetData(string format, bool autoConvert)
    {
        object data;

        if (format == DataFormats.CommaSeparatedValue)
        {
            var stringWriter = new StringWriter();
            Writer.Write(dataView, ',', "\r\n", stringWriter);
            var c = (char)0;
            stringWriter.Write(c);
            var s = stringWriter.ToString();
            data = new MemoryStream(Encoding.Default.GetBytes(s));
        }
        //else if (format == DataFormats.Html)
        //{
        //    var stringWriter = new StringWriter();
        //    HtmlFormatter.Write(_dataView, _columnIndexes, stringWriter);
        //    var htmlFragment = stringWriter.ToString();
        //    stringWriter = new StringWriter();
        //    WriteHtmlFragment(htmlFragment, stringWriter);
        //    var s = stringWriter.ToString();
        //    var bytes = Encoding.UTF8.GetBytes(s);
        //    data = new MemoryStream(bytes);
        //}
        else if (format == DataFormats.Text || format == DataFormats.UnicodeText)
        {
            data = dataView.ToStringTableString();
        }
        else if (format == "TabSeparatedValues")
        {
            // TODO
            data = "TabSep";
        }
        else
        {
            data = null;
        }

        return data;
    }

    bool IDataObject.GetDataPresent(Type format) => throw new NotImplementedException();

    bool IDataObject.GetDataPresent(string format) => throw new NotImplementedException();

    bool IDataObject.GetDataPresent(string format, bool autoConvert)
    {
        bool isDataPresent;

        if (format == DataFormats.CommaSeparatedValue ||
            format == DataFormats.Html ||
            format == DataFormats.Text ||
            format == DataFormats.UnicodeText)
        {
            isDataPresent = true;
        }
        else
        {
            isDataPresent = false;
        }

        return isDataPresent;
    }

    string[] IDataObject.GetFormats() => throw new NotImplementedException();

    string[] IDataObject.GetFormats(bool autoConvert) => [
            DataFormats.CommaSeparatedValue,
            DataFormats.Html,
            //DataFormats.StringFormat,
            DataFormats.Text,
            DataFormats.UnicodeText,
            "TabSeparatedValues" // TODO
        ];

    void IDataObject.SetData(object? data) => throw new NotImplementedException();
    void IDataObject.SetData(Type format, object? data) => throw new NotImplementedException();
    void IDataObject.SetData(string format, object? data) => throw new NotImplementedException();
    void IDataObject.SetData(string format, bool autoConvert, object? data) => throw new NotImplementedException();

    private static void WriteHtmlFragment(string htmlFragment, TextWriter textWriter)
    {
        var header = @"Version:0.9
StartHTML:{000000}
EndHTML:{111111}
StartFragment:{222222}
EndFragment:{333333}
";
        var startHtmlString = "<html><body><!--StartFragment-->";
        var endHtmlString = "<!--EndFragment--></body></html>";
        var startHtml = header.Length;
        var startFragment = startHtml + startHtmlString.Length;
        var htmlFragmentLength = Encoding.UTF8.GetByteCount(htmlFragment);
        var endFragment = startFragment + htmlFragmentLength;
        var endHtml = endFragment + endHtmlString.Length;

        header = header.Replace("{000000}", startHtml.ToString().PadLeft(8));
        header = header.Replace("{111111}", endHtml.ToString().PadLeft(8));
        header = header.Replace("{222222}", startFragment.ToString().PadLeft(8));
        header = header.Replace("{333333}", endFragment.ToString().PadLeft(8));

        textWriter.Write(header);
        textWriter.Write(startHtmlString);
        textWriter.Write(htmlFragment);
        textWriter.Write(endHtmlString);
    }
}