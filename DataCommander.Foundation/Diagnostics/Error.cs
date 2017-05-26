using System;

namespace Foundation.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Error
    {
        private readonly string message;
        private readonly Exception exception;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public Error( ErrorType type, string message, Exception exception )
        {
            this.Type = type;
            this.message = message;
            this.exception = exception;
        }

        /// <summary>
        /// 
        /// </summary>
        public ErrorType Type { get; }

        public override string ToString()
        {
            return $"{this.Type}\r\n{this.message}\r\n{this.exception}";
        }
    }
}