using System;
using System.Diagnostics;
using System.Text;

namespace Foundation.Diagnostics;

internal static class StackFrameExtensions
{
    public static string ToLogString(this StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        StringBuilder stringBuilder = new StringBuilder();
        System.Reflection.MethodBase method = frame.GetMethod();
        Type type = method.DeclaringType;
        string typeName = type.FullName;
        string name = method.Name;
        stringBuilder.AppendFormat("   at {0}.{1}(", typeName, name);
        System.Reflection.ParameterInfo[] parameters = method.GetParameters();

        for (int j = 0; j < parameters.Length; j++)
        {
            if (j > 0)
                stringBuilder.Append(',');

            System.Reflection.ParameterInfo parameter = parameters[j];
            type = parameter.ParameterType;
            typeName = type.Name;
            name = parameter.Name;

            stringBuilder.AppendFormat("{0} {1}", typeName, name);
        }

        stringBuilder.Append(')');
        stringBuilder.Append($" ILOffset: 0x{frame.GetILOffset().ToString("x")}, ");

        string fileName = frame.GetFileName();

        if (fileName != null)
        {
            int fileLineNumber = frame.GetFileLineNumber();
            int fileColumnNumber = frame.GetFileColumnNumber();
            stringBuilder.AppendFormat(" in {0}:line {1},column {2}", fileName, fileLineNumber, fileColumnNumber);
        }

        return stringBuilder.ToString();
    }
}