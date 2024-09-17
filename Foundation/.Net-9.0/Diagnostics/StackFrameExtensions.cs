using System;
using System.Diagnostics;
using System.Text;

namespace Foundation.Diagnostics;

internal static class StackFrameExtensions
{
    public static string ToLogString(this StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var stringBuilder = new StringBuilder();
        var method = frame.GetMethod();
        var type = method.DeclaringType;
        var typeName = type.FullName;
        var name = method.Name;
        stringBuilder.AppendFormat("   at {0}.{1}(", typeName, name);
        var parameters = method.GetParameters();

        for (var j = 0; j < parameters.Length; j++)
        {
            if (j > 0)
                stringBuilder.Append(',');

            var parameter = parameters[j];
            type = parameter.ParameterType;
            typeName = type.Name;
            name = parameter.Name;

            stringBuilder.AppendFormat("{0} {1}", typeName, name);
        }

        stringBuilder.Append(')');
        stringBuilder.Append($" ILOffset: 0x{frame.GetILOffset().ToString("x")}, ");

        var fileName = frame.GetFileName();

        if (fileName != null)
        {
            var fileLineNumber = frame.GetFileLineNumber();
            var fileColumnNumber = frame.GetFileColumnNumber();
            stringBuilder.AppendFormat(" in {0}:line {1},column {2}", fileName, fileLineNumber, fileColumnNumber);
        }

        return stringBuilder.ToString();
    }
}