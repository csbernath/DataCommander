using System;

namespace Foundation.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Error
    {
        private readonly string _message;
        private readonly Exception _exception;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public Error( ErrorType type, string message, Exception exception )
        {
            Type = type;
            _message = message;
            _exception = exception;
        }

        /// <summary>
        /// 
        /// </summary>
        public ErrorType Type { get; }

        public override string ToString()
        {
            return $"{Type}\r\n{_message}\r\n{_exception}";
        }
    }
}