using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Text;
using Foundation.Data.SqlClient;

namespace Foundation.Data
{
    public static class ExceptionExtensions
    {
        public static string ToLogString(this Exception e)
        {
            var sb = new StringBuilder();
            Append(sb, e);
            return sb.ToString();
        }

        private static void Append(StringBuilder sb, Win32Exception win32Exception)
        {
            sb.AppendLine(win32Exception.ToString());
            sb.AppendFormat("Win32Excpetion.NativeErrorCode: {0}", win32Exception.NativeErrorCode);
        }

        private static void Append(StringBuilder sb, SocketException socketException)
        {
            Win32Exception win32Exception = socketException;
            Append(sb, win32Exception);
            sb.AppendFormat("\r\nSocketException.SocketErrorCode: {0}", socketException.SocketErrorCode);
        }

        private static void Append(StringBuilder sb, SqlException sqlException)
        {
            sb.AppendLine(sqlException.ToString());
            var errors = sqlException.Errors.ToLogString();
            sb.Append(errors);
        }

        private static void Append(StringBuilder sb, Exception exception)
        {
            var current = exception;
            var first = true;

            while (current != null)
            {
                if (first)
                    first = false;
                else
                {
                    sb.AppendLine();
                    sb.AppendLine(new string('/', 80));
                }

                switch (current)
                {
                    case SocketException socketException:
                        Append(sb, socketException);
                        break;
                    case Win32Exception win32Exception:
                        Append(sb, win32Exception);
                        break;
                    case SqlException sqlException:
                        Append(sb, sqlException);
                        break;
                    default:
                        sb.Append(current);
                        break;
                }

                current = current.InnerException;
            }
        }
    }
}