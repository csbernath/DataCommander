namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using DataCommander.Foundation.Data.SqlClient;
    using DataCommander.Foundation.IO;

    /// <summary>
    /// 
    /// </summary>
    public static class MethodProfiler
    {
        /// <summary>
        /// 
        /// </summary>
        public const String ConditionString = "FOUNDATION_METHODPROFILER";

        private static MethodCollection methods = new MethodCollection();
        private static Dictionary<String, MethodFraction> methodFractions = new Dictionary<String, MethodFraction>();
        private static MethodInvocationStackCollection stacks = new MethodInvocationStackCollection();
        private static AsyncTextWriter textWriter;
        private static MethodFormatter methodFormatter = new MethodFormatter();
        private static MethodProfilerMethodInvocationFormatter methodProfilerMethodInvocationFormatter = new MethodProfilerMethodInvocationFormatter();

        static MethodProfiler()
        {
            Int64 beginTime = Stopwatch.GetTimestamp();
            DateTime now = OptimizedDateTime.Now;
            String applicationName;
            Assembly assembly = Assembly.GetEntryAssembly();

            if (assembly != null)
            {
                String codeBase = assembly.CodeBase;
                Uri uri = new Uri( codeBase );
                String fileName = uri.LocalPath;
                applicationName = fileName;
            }
            else
            {
                String baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                applicationName = baseDirectory;
            }

            String path = Path.GetTempFileName();
            StreamWriter streamWriter = new StreamWriter( path, false, Encoding.UTF8, 65536 );
            textWriter = new AsyncTextWriter( streamWriter );

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( @"declare @applicationId Int32

exec MethodProfilerApplication_Add {0},{1}",
                applicationName.ToTSqlNVarChar(),
                now.ToTSqlDateTime()
                );
            sb.AppendFormat( ",{0},{1}\r\n", beginTime, Stopwatch.Frequency );
            sb.Append( "set @applicationId    = @@identity\r\n" );
            textWriter.Write( sb.ToString() );
        }

        /// <summary>
        /// 
        /// </summary>
        [Conditional( ConditionString )]
        public static void BeginMethod()
        {
            Int64 beginTime = Stopwatch.GetTimestamp();
            Int32 threadId = Thread.CurrentThread.ManagedThreadId;
            StackTrace trace = new StackTrace( 1 );
            StackFrame frame = trace.GetFrame( 0 );
            MethodBase method = frame.GetMethod();
            Int32 methodId;
            Boolean added = false;

            lock (methods)
            {
                Boolean contains = methods.TryGetValue( method, out methodId );

                if (!contains)
                {
                    methodId = methods.Add( method );
                    added = true;
                }
            }

            if (added)
            {
                textWriter.Write( methodFormatter, method, methodId );
            }

            stacks.Push( threadId, methodId, beginTime );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        [Conditional( ConditionString )]
        public static void BeginMethodFraction( String name )
        {
            Int64 beginTime = Stopwatch.GetTimestamp();
            Int32 threadId = Thread.CurrentThread.ManagedThreadId;
            StackTrace trace = new StackTrace( 1 );
            StackFrame frame = trace.GetFrame( 0 );
            MethodBase method = frame.GetMethod();
            String key = MethodFraction.GetKey( method, name );
            MethodFraction methodFraction;
            Int32 methodId;
            Boolean added = false;

            lock (methods)
            {
                if (methodFractions.TryGetValue( key, out methodFraction ))
                {
                    methods.TryGetValue( methodFraction, out methodId );
                }
                else
                {
                    methodFraction = new MethodFraction( method, name );
                    methodFractions.Add( key, methodFraction );
                    methodId = methods.Add( methodFraction );
                    added = true;
                }
            }

            if (added)
            {
                textWriter.Write( methodFormatter, methodFraction, methodId );
            }

            stacks.Push( threadId, methodId, beginTime );
        }

        /// <summary>
        /// 
        /// </summary>
        [Conditional( ConditionString )]
        public static void EndMethod()
        {
            Int64 endTime = Stopwatch.GetTimestamp();
            Int32 threadId = Thread.CurrentThread.ManagedThreadId;
            StackTrace trace = new StackTrace( 1 );
            StackFrame frame = trace.GetFrame( 0 );
            MethodBase method = frame.GetMethod();
            Int32 methodId;
            methods.TryGetValue( method, out methodId );
            MethodInvocation item = stacks.Pop( threadId );

            if (item.MethodId != methodId)
            {
                throw new InvalidOperationException();
            }

            item.EndTime = endTime;
            textWriter.Write( methodProfilerMethodInvocationFormatter, item );
        }

        /// <summary>
        /// 
        /// </summary>
        [Conditional( ConditionString )]
        public static void EndMethodFraction()
        {
            Int64 endTime = Stopwatch.GetTimestamp();
            Int32 threadId = Thread.CurrentThread.ManagedThreadId;
            MethodInvocation item = stacks.Pop( threadId );
            item.EndTime = endTime;
            textWriter.Write( methodProfilerMethodInvocationFormatter, item );
        }

        /// <summary>
        /// 
        /// </summary>
        [Conditional( ConditionString )]
        public static void Close()
        {
            textWriter.Close();
        }
    }
}