namespace DataCommander.Foundation.Diagnostics.MethodProfiler
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
        public const string ConditionString = "FOUNDATION_METHODPROFILER";

        private static readonly MethodCollection methods = new MethodCollection();
        private static readonly Dictionary<string, MethodFraction> methodFractions = new Dictionary<string, MethodFraction>();
        private static readonly MethodInvocationStackCollection stacks = new MethodInvocationStackCollection();
        private static readonly AsyncTextWriter textWriter;
        private static readonly MethodFormatter methodFormatter = new MethodFormatter();
        private static readonly MethodProfilerMethodInvocationFormatter methodProfilerMethodInvocationFormatter = new MethodProfilerMethodInvocationFormatter();

        static MethodProfiler()
        {
            var beginTime = Stopwatch.GetTimestamp();
            var now = LocalTime.Default.Now;
            string applicationName;
            var assembly = Assembly.GetEntryAssembly();

            if (assembly != null)
            {
                var codeBase = assembly.CodeBase;
                var uri = new Uri( codeBase );
                var fileName = uri.LocalPath;
                applicationName = fileName;
            }
            else
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                applicationName = baseDirectory;
            }

            var path = Path.GetTempFileName();
            var streamWriter = new StreamWriter( path, false, Encoding.UTF8, 65536 );
            textWriter = new AsyncTextWriter( streamWriter );

            var sb = new StringBuilder();
            sb.AppendFormat( @"declare @applicationId int

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
            var beginTime = Stopwatch.GetTimestamp();
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var trace = new StackTrace( 1 );
            var frame = trace.GetFrame( 0 );
            var method = frame.GetMethod();
            int methodId;
            var added = false;

            lock (methods)
            {
                var contains = methods.TryGetValue( method, out methodId );

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
        public static void BeginMethodFraction( string name )
        {
            var beginTime = Stopwatch.GetTimestamp();
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var trace = new StackTrace( 1 );
            var frame = trace.GetFrame( 0 );
            var method = frame.GetMethod();
            var key = MethodFraction.GetKey( method, name );
            MethodFraction methodFraction;
            int methodId;
            var added = false;

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
            var endTime = Stopwatch.GetTimestamp();
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var trace = new StackTrace( 1 );
            var frame = trace.GetFrame( 0 );
            var method = frame.GetMethod();
            int methodId;
            methods.TryGetValue( method, out methodId );
            var item = stacks.Pop( threadId );

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
            var endTime = Stopwatch.GetTimestamp();
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var item = stacks.Pop( threadId );
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