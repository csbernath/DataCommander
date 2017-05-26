using System;
using Foundation.Collections;

namespace Foundation.Diagnostics.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public static class FoundationContract
    {
        private enum ExceptionType
        {
            None,
            ArgumentException,
            ArgumentNullException,
            ArgumentOutOfRangeException
        }

        private static readonly TypeDictionary<ExceptionType> typeDictionary = new TypeDictionary<ExceptionType>();

        static FoundationContract()
        {
            typeDictionary.Add<ArgumentException>(ExceptionType.ArgumentException);
            typeDictionary.Add<ArgumentNullException>(ExceptionType.ArgumentNullException);
            typeDictionary.Add<ArgumentOutOfRangeException>(ExceptionType.ArgumentOutOfRangeException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static void Assert(bool condition)
        {
            Assert(condition, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="userMessage"></param>
        public static void Assert(bool condition, string userMessage)
        {
            if (!condition)
                throw new InvalidOperationException(userMessage);
        }

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
                var exceptionType = typeDictionary.GetValueOrDefault<TException>();
                switch (exceptionType)
                {
                    case ExceptionType.ArgumentException:
                        throw new ArgumentException(userMessage);

                    case ExceptionType.ArgumentNullException:
                        throw new ArgumentNullException(userMessage, (Exception)null);

                    case ExceptionType.ArgumentOutOfRangeException:
                        throw new ArgumentOutOfRangeException(userMessage, (Exception)null);

                    default:
                        throw new Exception(userMessage);
                }
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