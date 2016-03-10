namespace DataCommander.Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class Error
    {
        private readonly ErrorType type;
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
            this.type = type;
            this.message = message;
            this.exception = exception;
        }

        /// <summary>
        /// 
        /// </summary>
        public ErrorType Type => this.type;

        public override string ToString()
        {
            return $"{this.type}\r\n{this.message}\r\n{this.exception}";
        }
    }
}