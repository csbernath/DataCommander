using System;
using System.Security.Cryptography;

namespace Foundation.Data;

public static class RandomNumberGeneratorExtensions
{
    public static long GetInt64(this RandomNumberGenerator randomNumberGenerator)
    {
        ArgumentNullException.ThrowIfNull(randomNumberGenerator);
        byte[] data = new byte[8];
        randomNumberGenerator.GetBytes(data);
        long randomNumber = BitConverter.ToInt64(data, 0);
        return randomNumber;
    }
}