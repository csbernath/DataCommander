namespace DataCommander.Foundation.Diagnostics.Contracts
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static class FoundationContract
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="condition"></param>
        public static void Requires<TException>(bool condition) where TException : Exception
        {
            Requires<TException>(condition, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="condition"></param>
        /// <param name="userMessage"></param>
        public static void Requires<TException>(bool condition, string userMessage) where TException : Exception
        {
            if (!condition)
            {
                Selection.CreateTypeIsSelection(typeof (TException))
                    .IfTypeIs<ArgumentNullException>(() =>
                    {
                        throw new ArgumentNullException(userMessage, (Exception)null);
                    })
                    .IfTypeIs<ArgumentOutOfRangeException>(() =>
                    {
                        throw new ArgumentOutOfRangeException(userMessage, (Exception)null);
                    })
                    .Else(() =>
                    {
                        throw new Exception(userMessage);
                    });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        public static void Ensures(bool condition)
        {
        }
    }
}