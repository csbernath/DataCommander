using System;
using System.Globalization;
using System.IO;
using Foundation.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Text
{
    public static class StringHelper
    {
        public static string FormatColumn(string col, int colWidth, bool alignRight)
        {
            var length = col != null
                ? col.Length
                : 0;
            var spaceLen = colWidth - length;
            string formatted;

            if (spaceLen >= 0)
            {
                var space = new string(' ', spaceLen);

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
                formatted = col.Substring(0, colWidth);
            }

            return formatted;
        }

        public static unsafe void SetChar(string s, int index, char ch)
        {
            FoundationContract.Requires<ArgumentException>(index >= 0);
            FoundationContract.Requires<ArgumentException>(index < s.Length);

            fixed (char* p = s)
            {
                p[index] = ch;
            }
        }

        public static unsafe void ToLower(string s)
        {
            fixed (char* pfixed = s)
            {
                for (var p = pfixed; *p != 0; p++)
                {
                    *p = char.ToLower(*p, CultureInfo.CurrentCulture);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public static unsafe void ToUpper(string s)
        {
            fixed (char* pfixed = s)
            {
                for (var p = pfixed; *p != 0; p++)
                {
                    *p = char.ToUpper(*p, CultureInfo.CurrentCulture);
                }
            }
        }

        public static void WriteMethod(TextWriter textWriter, object obj, string methodName, params object[] parameters)
        {
            Assert.IsNotNull(textWriter);
            Assert.IsNotNull(obj);

            var type = obj.GetType();
            var methodInfo = type.GetMethod(methodName);
            var parameterInfos = methodInfo.GetParameters();
            var typeName = TypeNameCollection.GetTypeName(methodInfo.ReturnType);
            var line = typeName + " " + methodName + "(" + Environment.NewLine;
            var length = Math.Min(parameters.Length, parameterInfos.Length);

            for (var i = 0; i < length; i++)
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
            var b = !string.IsNullOrEmpty(value)
                ? bool.Parse(value)
                : nullValue;
            return b;
        }
    }
}