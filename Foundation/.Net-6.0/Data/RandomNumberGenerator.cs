using System;
using System.Security.Cryptography;
using Foundation.Assertions;

namespace Foundation.Data
{
    public static class RandomNumberGeneratorExtensions
    {
        public static long GetInt64(this RandomNumberGenerator randomNumberGenerator)
        {
            Assert.IsNotNull(randomNumberGenerator);
            var data = new byte[8];
            randomNumberGenerator.GetBytes(data);
            var randomNumber = BitConverter.ToInt64(data, 0);
            return randomNumber;
        }
    }
}