namespace DataCommander.Foundation.Diagnostics
{
    using System;

    internal sealed class TextLogFormatter : ILogFormatter
    {
        internal static string Format( LogEntry entry )
        {
            Char logLevelChar;
            switch (entry.LogLevel)
            {
                case LogLevel.Debug:
                    logLevelChar = 'D';
                    break;

                case LogLevel.Error:
                    logLevelChar = 'E';
                    break;

                case LogLevel.Information:
                    logLevelChar = 'I';
                    break;

                case LogLevel.Trace:
                    logLevelChar = 'T';
                    break;

                case LogLevel.Warning:
                    logLevelChar = 'W';
                    break;

                default:
                    logLevelChar = '?';
                        break;
            }

            return string.Format(
                "[{0}|{1}|{2}|{3},{4}|{5}] {6}\r\n",
                entry.CreationTime.ToString( "HH:mm:ss.fff" ),
                entry.Id,
                logLevelChar,
                entry.ThreadName,
                entry.ManagedThreadId,
                entry.LogName,
                entry.Message );

                //creationTime.EnvironmentTickCount,
                //creationTime.StopwatchTimestamp );

            //string userName = entry.UserName;

            //if (userName != null)
            //{
            //    sb.Append( ' ' );
            //    sb.Append( userName.PadRight( 11, ' ' ) );
            //}

            //string hostName = entry.HostName;

            //if (hostName != null)
            //{
            //    sb.Append( ' ' );

            //    if (logLevel == LogLevel.Trace)
            //    {
            //        sb.Append( hostName );
            //    }
            //    else
            //    {
            //        sb.Append( hostName.PadRight( 15, ' ' ) );
            //    }
            //}
        }

        //internal static string Format2( LogEntry entry )
        //{
        //    Char logLevelChar;
        //    switch (entry.LogLevel)
        //    {
        //        case LogLevel.Debug:
        //            logLevelChar = 'D';
        //            break;

        //        case LogLevel.Error:
        //            logLevelChar = 'E';
        //            break;

        //        case LogLevel.Information:
        //            logLevelChar = 'I';
        //            break;

        //        case LogLevel.Trace:
        //            logLevelChar = 'T';
        //            break;

        //        case LogLevel.Warning:
        //            logLevelChar = 'W';
        //            break;

        //        default:
        //            logLevelChar = '?';
        //            break;
        //    }

        //    var sb = new StringBuilder( 4096 );
        //    sb.Append( '[' );
        //    sb.Append( entry.CreationTime.ToString( "HH:mm:ss.fff" ) );
        //    sb.Append( '|' );
        //    sb.Append( entry.Id );
        //    sb.Append( '|' );
        //    sb.Append( logLevelChar );
        //    sb.Append( '|' );
        //    sb.Append( entry.ThreadName );
        //    sb.Append( ',' );
        //    sb.Append( entry.ManagedThreadId );
        //    sb.Append( '|' );
        //    sb.Append( entry.LogName );
        //    sb.Append( "] " );
        //    sb.Append( entry.Message );
        //    sb.Append( "\r\n" );

        //    return sb.ToString();
        //}

        string ILogFormatter.Begin()
        {
            return null;
        }

        string ILogFormatter.Format( LogEntry entry )
        {
            return Format( entry );
        }

        string ILogFormatter.End()
        {
            return null;
        }
    }
}