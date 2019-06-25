using System;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Core;
using Foundation.Text;

namespace Foundation.Data.DbQueryBuilding
{
    internal static class CSharpExtensions
    {
        public static IDisposable AddBlock(this IndentedTextBuilder indentedTextBuilder)
        {
            indentedTextBuilder.Add("{");
            var indentation = indentedTextBuilder.Indent(1);

            return new Disposer(() =>
            {
                indentation.Dispose();
                indentedTextBuilder.Add("}");
            });
        }
    }

    public static class DataTransferObjectFactory
    {
        public static ReadOnlyCollection<IndentedLine> CreateDataTransferObject(string name, ReadOnlyCollection<DataTransferObjectField> fields)
        {
            var indentedTextBuilder = new IndentedTextBuilder();

            indentedTextBuilder.Add($"public sealed class {name}");
            using (indentedTextBuilder.AddBlock())
            {
                foreach (var field in fields)
                    indentedTextBuilder.Add($"public readonly {field.Type} {field.Name};");

                indentedTextBuilder.Add();
                var constructor = GetConstructor(name, fields);
                indentedTextBuilder.Add(constructor);
            }

            return indentedTextBuilder.ToReadOnlyCollection();
        }

        private static ReadOnlyCollection<IndentedLine> GetConstructor(string name, ReadOnlyCollection<DataTransferObjectField> fields)
        {
            var indentedTextBuilder = new IndentedTextBuilder();

            var parameters = fields.Select(field => $"{field.Type} {ToCamelCase(field.Name)}").Join(", ");
            indentedTextBuilder.Add($"public {name}({parameters})");
            using (indentedTextBuilder.AddBlock())
            {
                foreach (var field in fields)
                    indentedTextBuilder.Add($"{field.Name} = {ToCamelCase(field.Name)};");
            }

            return indentedTextBuilder.ToReadOnlyCollection();
        }

        private static string ToCamelCase(string pascalCase)
        {
            return !pascalCase.IsNullOrEmpty()
                ? char.ToLower(pascalCase[0]) + pascalCase.Substring(1)
                : pascalCase;
        }

        private static string ToPascalCase(string camelCase)
        {
            return !camelCase.IsNullOrEmpty()
                ? char.ToUpper(camelCase[0]) + camelCase.Substring(1)
                : camelCase;
        }
    }
}