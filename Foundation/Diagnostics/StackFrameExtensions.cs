using System;
using System.Diagnostics;
using System.Text;

namespace Foundation.Diagnostics;

internal static class StackFrameExtensions
{
    extension(StackFrame frame)
    {
        public string ToLogString()
        {
            ArgumentNullException.ThrowIfNull(frame);

            var stringBuilder = new StringBuilder();
            var method = frame.GetMethod()!;
            var type = method.DeclaringType!;
            var typeName = type.FullName;
            var name = method.Name;
            stringBuilder.Append($"   at {typeName}.{name}(");
            var parameters = method.GetParameters();

            for (var j = 0; j < parameters.Length; j++)
            {
                if (j > 0)
                    stringBuilder.Append(',');

                var parameter = parameters[j];
                type = parameter.ParameterType;
                typeName = type.Name;
                name = parameter.Name;

                stringBuilder.Append($"{typeName} {name}");
            }

            stringBuilder.Append(')');
            stringBuilder.Append($" ILOffset: 0x{frame.GetILOffset().ToString("x")}, ");

            var fileName = frame.GetFileName();
            if (fileName != null)
            {
                var fileLineNumber = frame.GetFileLineNumber();
                var fileColumnNumber = frame.GetFileColumnNumber();
                stringBuilder.Append($" in {fileName}:line {fileLineNumber},column {fileColumnNumber}");
            }

            return stringBuilder.ToString();
        }
    }
}