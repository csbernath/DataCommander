namespace DataCommander.Foundation.Text
{
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public static class MyStringBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringBuilder MyAppendFormat(
            this StringBuilder source,
            IFormatProvider provider, string format, params Func<object>[] args)
        {
            if (format == null || args == null)
            {
                throw new ArgumentNullException((format == null) ? "format" : "args");
            }
            var num = 0;
            var length = format.Length;
            var c = '\0';
            ICustomFormatter customFormatter = null;
            if (provider != null)
            {
                customFormatter = (ICustomFormatter)provider.GetFormat(typeof(ICustomFormatter));
            }
            while (true)
            {
                if (num < length)
                {
                    c = format[num];
                    num++;
                    if (c == '}')
                    {
                        if (num < length && format[num] == '}')
                        {
                            num++;
                        }
                        else
                        {
                            // StringBuilder.FormatError();
                            throw new FormatException();
                        }
                    }
                    if (c == '{')
                    {
                        if (num >= length || format[num] != '{')
                        {
                            num--;
                            goto IL_9A;
                        }
                        num++;
                    }
                    source.Append(c);
                    continue;
                }
            IL_9A:
                if (num == length)
                {
                    return source;
                }
                num++;
                if (num == length || (c = format[num]) < '0' || c > '9')
                {
                    //StringBuilder.FormatError();
                    throw new FormatException();
                }
                var num2 = 0;
                do
                {
                    num2 = num2 * 10 + (int)c - 48;
                    num++;
                    if (num == length)
                    {
                        //StringBuilder.FormatError();
                        throw new FormatException();
                    }
                    c = format[num];
                }
                while (c >= '0' && c <= '9' && num2 < 1000000);
                if (num2 >= args.Length)
                {
                    Trace.WriteLine(string.Format("ERROR: num2 >= args.Length, {0} >= {1}, args[{0}] is missing", num2, args.Length));
                    break;
                }
                while (num < length && (c = format[num]) == ' ')
                {
                    num++;
                }
                var flag = false;
                var num3 = 0;
                if (c == ',')
                {
                    num++;
                    while (num < length && format[num] == ' ')
                    {
                        num++;
                    }
                    if (num == length)
                    {
                        //StringBuilder.FormatError();
                        throw new FormatException();
                    }
                    c = format[num];
                    if (c == '-')
                    {
                        flag = true;
                        num++;
                        if (num == length)
                        {
                            //StringBuilder.FormatError();
                            throw new FormatException();
                        }
                        c = format[num];
                    }
                    if (c < '0' || c > '9')
                    {
                        //StringBuilder.FormatError();
                        throw new FormatException();
                    }
                    do
                    {
                        num3 = num3 * 10 + (int)c - 48;
                        num++;
                        if (num == length)
                        {
                            //StringBuilder.FormatError();
                            throw new FormatException();
                        }
                        c = format[num];
                        if (c < '0' || c > '9')
                        {
                            break;
                        }
                    }
                    while (num3 < 1000000);
                }
                while (num < length && (c = format[num]) == ' ')
                {
                    num++;
                }
                object obj = args[num2];
                StringBuilder stringBuilder = null;
                if (c == ':')
                {
                    num++;
                    while (true)
                    {
                        if (num == length)
                        {
                            //StringBuilder.FormatError();
                            throw new FormatException();
                        }
                        c = format[num];
                        num++;
                        if (c == '{')
                        {
                            if (num < length && format[num] == '{')
                            {
                                num++;
                            }
                            else
                            {
                                //StringBuilder.FormatError();
                                throw new FormatException();
                            }
                        }
                        else
                        {
                            if (c == '}')
                            {
                                if (num >= length || format[num] != '}')
                                {
                                    break;
                                }
                                num++;
                            }
                        }
                        if (stringBuilder == null)
                        {
                            stringBuilder = new StringBuilder();
                        }
                        stringBuilder.Append(c);
                    }
                    num--;
                }
                if (c != '}')
                {
                    //StringBuilder.FormatError();
                    throw new FormatException();
                }
                num++;
                string text = null;
                string text2 = null;
                if (customFormatter != null)
                {
                    if (stringBuilder != null)
                    {
                        text = stringBuilder.ToString();
                    }
                    text2 = customFormatter.Format(text, obj, provider);
                }
                if (text2 == null)
                {
                    var formattable = obj as IFormattable;
                    if (formattable != null)
                    {
                        if (text == null && stringBuilder != null)
                        {
                            text = stringBuilder.ToString();
                        }
                        text2 = formattable.ToString(text, provider);
                    }
                    else
                    {
                        if (obj != null)
                        {
                            text2 = obj.ToString();
                        }
                    }
                }
                if (text2 == null)
                {
                    text2 = string.Empty;
                }
                var num4 = num3 - text2.Length;
                if (!flag && num4 > 0)
                {
                    source.Append(' ', num4);
                }
                source.Append(text2);
                if (flag && num4 > 0)
                {
                    source.Append(' ', num4);
                }
            }
            //throw new FormatException( Environment.GetResourceString( "Format_IndexOutOfRange" ) );
            throw new FormatException("Format_IndexOutOfRange");
        }
    }
}