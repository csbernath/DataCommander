﻿using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Text;
using Foundation.Data.SqlClient;

namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="sb"></param>
        private static void Append(StringBuilder sb, Exception exception)
        {
            var current = exception;
            var first = true;

            while (current != null)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.AppendLine();
                    sb.AppendLine(new string('/', 80));
                }

                var socketException = current as SocketException;
                if (socketException != null)
                {
                    Append(sb, socketException);
                }
                else
                {
                    var win32Exception = current as Win32Exception;
                    if (win32Exception != null)
                    {
                        Append(sb, win32Exception);
                    }
                    else
                    {
                        var sqlException = current as SqlException;
                        if (sqlException != null)
                        {
                            Append(sb, sqlException);
                        }
                        else
                        {
                            sb.Append(current);
                        }
                    }
                }

                current = current.InnerException;
            }
        }
    }
}