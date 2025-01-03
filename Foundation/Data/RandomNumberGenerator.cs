using System;
using System.Security.Cryptography;

namespace Foundation.Data;

public static class RandomNumberGeneratorExtensions
{
    public static long GetInt64(this RandomNumberGenerator randomNumberGenerator)
    {
        ArgumentNullException.ThrowIfNull(randomNumberGenerator);
        var data = new byte[8];
        randomNumberGenerator.GetBytes(data);
        var randomNumber = BitConverter.ToInt64(data, 0);
        return randomNumber;
    }
}