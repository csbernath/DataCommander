namespace DataCommander.Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class Error
    {
        private readonly ErrorType type;
        private readonly String message;
        private readonly Exception exception;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public Error( ErrorType type, String message, Exception exception )
        {
            this.type = type;
            this.message = message;
            this.exception = exception;
        }

        /// <summary>
        /// 
        /// </summary>
        public ErrorType Type
        {
            get
            {
                return this.type;
            }
        }

        public override String ToString()
        {
            return String.Format( "{0}\r\n{1}\r\n{2}", this.type, this.message, this.exception );
        }
    }
}