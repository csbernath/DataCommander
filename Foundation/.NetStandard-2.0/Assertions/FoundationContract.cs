using System;
using Foundation.Assertions;

namespace Foundation.Diagnostics.Contracts
{
    public static class FoundationContract
    {
        private enum ExceptionType
        {
            None,
            ArgumentException,
            ArgumentNullException,
            ArgumentOutOfRangeException,
            InvalidOperationException
        }

        private static readonly TypeDictionary<ExceptionType> TypeDictionary = new TypeDictionary<ExceptionType>();

        static FoundationContract()
        {
            TypeDictionary.Add<ArgumentException>(ExceptionType.ArgumentException);
            TypeDictionary.Add<ArgumentNullException>(ExceptionType.ArgumentNullException);
            TypeDictionary.Add<ArgumentOutOfRangeException>(ExceptionType.ArgumentOutOfRangeException);
            TypeDictionary.Add<InvalidOperationException>(ExceptionType.ArgumentOutOfRangeException);
        }

        public static void Assert(bool condition) => Assert(condition, null);

        public static void Assert(bool condition, string userMessage)
        {
            if (!condition)
                throw new InvalidOperationException(userMessage);
        }

        public static void Requires<TException>(bool condition) where TException : Exception => Requires<TException>(condition, null);

        public static void Requires<TException>(bool condition, string userMessage) where TException : Exception
        {
            if (!condition)
            {
                var exceptionType = TypeDictionary.GetValueOrDefault<TException>();
                switch (exceptionType)
                {
                    case ExceptionType.ArgumentException:
                        throw new ArgumentException(userMessage);

                    case ExceptionType.ArgumentNullException:
                        throw new ArgumentNullException(userMessage, (Exception) null);

                    case ExceptionType.ArgumentOutOfRangeException:
                        throw new ArgumentOutOfRangeException(userMessage, (Exception) null);

                    case ExceptionType.InvalidOperationException:
                        throw new InvalidOperationException(userMessage);

                    default:
                        throw new Exception(userMessage);
                }
            }
        }

        public static void Ensures(bool condition)
        {
        }
    }
}