using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Foundation.Core;
using Foundation.Data.SqlClient;
using Foundation.IO;

namespace Foundation.Data.MethodProfiler;

public static class MethodProfiler
{
    private const string ConditionString = "FOUNDATION_METHODPROFILER";

    private static readonly MethodCollection Methods = [];
    private static readonly Dictionary<string, MethodFraction> MethodFractions = [];
    private static readonly MethodInvocationStackCollection Stacks = new();
    private static readonly AsyncTextWriter TextWriter;
    private static readonly MethodFormatter MethodFormatter = new();
    private static readonly MethodProfilerMethodInvocationFormatter MethodProfilerMethodInvocationFormatter = new();

    static MethodProfiler()
    {
        long beginTime = Stopwatch.GetTimestamp();
        DateTime now = LocalTime.Default.Now;
        string applicationName;
        Assembly assembly = Assembly.GetEntryAssembly();

        if (assembly != null)
        {
            string location = assembly.Location;
            Uri uri = new Uri(location);
            string fileName = uri.LocalPath;
            applicationName = fileName;
        }
        else
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            applicationName = baseDirectory;
        }

        string path = Path.GetTempFileName();
        StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8, 65536);
        TextWriter = new AsyncTextWriter(streamWriter);

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(@"declare @applicationId int

exec MethodProfilerApplication_Add {0},{1}",
            applicationName.ToNullableNVarChar(),
            now.ToSqlConstant()
        );
        sb.AppendFormat(",{0},{1}\r\n", beginTime, Stopwatch.Frequency);
        sb.Append("set @applicationId    = @@identity\r\n");
        TextWriter.Write(sb.ToString());
    }

    [Conditional(ConditionString)]
    public static void BeginMethod()
    {
        long beginTime = Stopwatch.GetTimestamp();
        int threadId = Environment.CurrentManagedThreadId;
        StackTrace trace = new StackTrace(1);
        StackFrame frame = trace.GetFrame(0);
        MethodBase method = frame.GetMethod();
        int methodId;
        bool added = false;

        lock (Methods)
        {
            bool contains = Methods.TryGetValue(method, out methodId);

            if (!contains)
            {
                methodId = Methods.Add(method);
                added = true;
            }
        }

        if (added)
            TextWriter.Write(MethodFormatter, method, methodId);

        Stacks.Push(threadId, methodId, beginTime);
    }

    [Conditional(ConditionString)]
    public static void BeginMethodFraction(string name)
    {
        long beginTime = Stopwatch.GetTimestamp();
        int threadId = Environment.CurrentManagedThreadId;
        StackTrace trace = new StackTrace(1);
        StackFrame frame = trace.GetFrame(0);
        MethodBase method = frame.GetMethod();
        string key = MethodFraction.GetKey(method, name);
        MethodFraction methodFraction;
        int methodId;
        bool added = false;

        lock (Methods)
        {
            if (MethodFractions.TryGetValue(key, out methodFraction))
                Methods.TryGetValue(methodFraction, out methodId);
            else
            {
                methodFraction = new MethodFraction(method, name);
                MethodFractions.Add(key, methodFraction);
                methodId = Methods.Add(methodFraction);
                added = true;
            }
        }

        if (added)
            TextWriter.Write(MethodFormatter, methodFraction, methodId);

        Stacks.Push(threadId, methodId, beginTime);
    }

    [Conditional(ConditionString)]
    public static void EndMethod()
    {
        long endTime = Stopwatch.GetTimestamp();
        int threadId = Environment.CurrentManagedThreadId;
        StackTrace trace = new StackTrace(1);
        StackFrame frame = trace.GetFrame(0);
        MethodBase method = frame.GetMethod();
        Methods.TryGetValue(method, out int methodId);
        MethodInvocation item = Stacks.Pop(threadId);

        if (item.MethodId != methodId)
            throw new InvalidOperationException();

        item.EndTime = endTime;
        TextWriter.Write(MethodProfilerMethodInvocationFormatter, item);
    }

    [Conditional(ConditionString)]
    public static void EndMethodFraction()
    {
        long endTime = Stopwatch.GetTimestamp();
        int threadId = Environment.CurrentManagedThreadId;
        MethodInvocation item = Stacks.Pop(threadId);
        item.EndTime = endTime;
        TextWriter.Write(MethodProfilerMethodInvocationFormatter, item);
    }

    [Conditional(ConditionString)]
    public static void Close() => TextWriter.Close();
}