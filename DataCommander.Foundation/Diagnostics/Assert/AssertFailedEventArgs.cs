#if FOUNDATION_3_5
namespace DataCommander.Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class AssertFailedEventArgs : EventArgs
    {
        private readonly AssertMessage message;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public AssertFailedEventArgs( AssertMessage message )
        {
            this.message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        public AssertMessage Message
        {
            get
            {
                return this.message;
            }
        }
    }
}
#endif