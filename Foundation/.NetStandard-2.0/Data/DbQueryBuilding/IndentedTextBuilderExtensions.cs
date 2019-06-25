using System;
using Foundation.Core;
using Foundation.Text;

namespace Foundation.Data.DbQueryBuilding
{
    public static class IndentedTextBuilderExtensions
    {
        public static IDisposable AddBlock(this IndentedTextBuilder indentedTextBuilder, string begin, string end)
        {
            indentedTextBuilder.Add(begin);
            var indentation = indentedTextBuilder.Indent(1);

            return new Disposer(() =>
            {
                indentation.Dispose();
                indentedTextBuilder.Add(end);
            });
        }

        public static IDisposable AddCSharpBlock(this IndentedTextBuilder indentedTextBuilder) => indentedTextBuilder.AddBlock("{", "}");
    }
}