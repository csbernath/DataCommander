using System;
using System.Globalization;
using System.IO;
using Foundation.Assertions;
using Foundation.Collections;

namespace Foundation.Text;

public static class StringHelper
{
    public static string FormatColumn(string col, int colWidth, bool alignRight)
    {
        int length = col != null
            ? col.Length
            : 0;
        int spaceLen = colWidth - length;
        string formatted;

        if (spaceLen >= 0)
        {
            string space = new string(' ', spaceLen);

            if (alignRight)
            {
                formatted = space + col;
            }
            else
            {
                formatted = col + space;
            }
        }
        else
        {
            formatted = col![..colWidth];
        }

        return formatted;
    }

    public static unsafe void SetChar(string s, int index, char ch)
    {
        Assert.IsTrue(index >= 0);
        Assert.IsTrue(index < s.Length);

        fixed (char* p = s)
        {
            p[index] = ch;
        }
    }

    public static unsafe void ToLower(string s)
    {
        fixed (char* pfixed = s)
        {
            for (char* p = pfixed; *p != 0; p++)
            {
                *p = char.ToLower(*p, CultureInfo.CurrentCulture);
            }
        }
    }

    public static unsafe void ToUpper(string s)
    {
        fixed (char* pfixed = s)
        {
            for (char* p = pfixed; *p != 0; p++)
            {
                *p = char.ToUpper(*p, CultureInfo.CurrentCulture);
            }
        }
    }

    public static void WriteMethod(TextWriter textWriter, object obj, string methodName, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(textWriter);
        ArgumentNullException.ThrowIfNull(obj);

        Type type = obj.GetType();
        System.Reflection.MethodInfo? methodInfo = type.GetMethod(methodName);
        System.Reflection.ParameterInfo[] parameterInfos = methodInfo!.GetParameters();
        string typeName = TypeNameCollection.GetTypeName(methodInfo.ReturnType);
        string line = typeName + " " + methodName + "(" + Environment.NewLine;
        int length = Math.Min(parameters.Length, parameterInfos.Length);

        for (int i = 0; i < length; i++)
        {
            typeName = TypeNameCollection.GetTypeName(parameterInfos[i].ParameterType);

            line +=
                "  " + typeName + " " +
                parameterInfos[i].Name + " = " +
                parameters[i];

            if (i < length - 1)
                line += "," + Environment.NewLine;
        }

        line += ')';

        textWriter.Write(line);
    }

    public static bool ParseBoolean(string value, bool nullValue)
    {
        bool b = !string.IsNullOrEmpty(value)
            ? bool.Parse(value)
            : nullValue;
        return b;
    }
}