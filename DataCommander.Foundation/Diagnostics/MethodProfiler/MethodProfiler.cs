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
        public const string ConditionString = "FOUNDATION_METHODPROFILER";

        private static readonly MethodCollection methods = new MethodCollection();
        private static readonly Dictionary<string, MethodFraction> methodFractions = new Dictionary<string, MethodFraction>();
        private static readonly MethodInvocationStackCollection stacks = new MethodInvocationStackCollection();
        private static readonly AsyncTextWriter textWriter;
        private static readonly MethodFormatter methodFormatter = new MethodFormatter();
        private static readonly MethodProfilerMethodInvocationFormatter methodProfilerMethodInvocationFormatter = new MethodProfilerMethodInvocationFormatter();

        static MethodProfiler()
        {
            long beginTime = Stopwatch.GetTimestamp();
            DateTime now = LocalTime.Default.Now;
            string applicationName;
            Assembly assembly = Assembly.GetEntryAssembly();

            if (assembly != null)
            {
                string codeBase = assembly.CodeBase;
                Uri uri = new Uri( codeBase );
                string fileName = uri.LocalPath;
                applicationName = fileName;
            }
            else
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                applicationName = baseDirectory;
            }

            string path = Path.GetTempFileName();
            StreamWriter streamWriter = new StreamWriter( path, false, Encoding.UTF8, 65536 );
            textWriter = new AsyncTextWriter( streamWriter );

            StringBuilder sb = new StringBuilder();
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
            long beginTime = Stopwatch.GetTimestamp();
            int threadId = Thread.CurrentThread.ManagedThreadId;
            StackTrace trace = new StackTrace( 1 );
            StackFrame frame = trace.GetFrame( 0 );
            MethodBase method = frame.GetMethod();
            int methodId;
            bool added = false;

            lock (methods)
            {
                bool contains = methods.TryGetValue( method, out methodId );

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
            long beginTime = Stopwatch.GetTimestamp();
            int threadId = Thread.CurrentThread.ManagedThreadId;
            StackTrace trace = new StackTrace( 1 );
            StackFrame frame = trace.GetFrame( 0 );
            MethodBase method = frame.GetMethod();
            string key = MethodFraction.GetKey( method, name );
            MethodFraction methodFraction;
            int methodId;
            bool added = false;

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
            long endTime = Stopwatch.GetTimestamp();
            int threadId = Thread.CurrentThread.ManagedThreadId;
            StackTrace trace = new StackTrace( 1 );
            StackFrame frame = trace.GetFrame( 0 );
            MethodBase method = frame.GetMethod();
            int methodId;
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
            long endTime = Stopwatch.GetTimestamp();
            int threadId = Thread.CurrentThread.ManagedThreadId;
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