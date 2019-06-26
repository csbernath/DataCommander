using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Core;
using Foundation.Text;

namespace Foundation.Data.DbQueryBuilding
{
    public static class DataTransferObjectFactory
    {
        public static ReadOnlyCollection<Line> CreateDataTransferObject(string name, ReadOnlyCollection<DataTransferObjectField> fields)
        {
            var textBuilder = new TextBuilder();

            textBuilder.Add($"public sealed class {name}");
            using (textBuilder.AddCSharpBlock())
            {
                foreach (var field in fields)
                    textBuilder.Add($"public readonly {field.Type} {field.Name};");

                textBuilder.Add(Line.Empty);
                var constructor = GetConstructor(name, fields);
                textBuilder.Add(constructor);
            }

            return textBuilder.ToReadOnlyCollection();
        }

        private static ReadOnlyCollection<Line> GetConstructor(string name, ReadOnlyCollection<DataTransferObjectField> fields)
        {
            var textBuilder = new TextBuilder();

            var parameters = fields.Select(field => $"{field.Type} {ToCamelCase(field.Name)}").Join(", ");
            textBuilder.Add($"public {name}({parameters})");
            using (textBuilder.AddCSharpBlock())
            {
                foreach (var field in fields)
                    textBuilder.Add($"{field.Name} = {ToCamelCase(field.Name)};");
            }

            return textBuilder.ToReadOnlyCollection();
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