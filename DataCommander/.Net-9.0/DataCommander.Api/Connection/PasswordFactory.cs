using System;
using System.Security.Cryptography;
using System.Text;

namespace DataCommander.Api.Connection;

public static class PasswordFactory
{
    private static readonly byte[] Entropy = [0x56, 0x4f, 0x3d, 0x78, 0xf1];

    public static Password CreateFromPlainText(string plainText)
    {
        byte[] @protected = Protect(plainText);
        System.Security.SecureString secureString = SecureStringFactory.CreateFromPlainText(plainText);
        return new Password(@protected, secureString);
    }

    public static Password CreateFromProtected(byte[] @protected)
    {
        string plainText = Unprotect(@protected);
        System.Security.SecureString secureString = SecureStringFactory.CreateFromPlainText(plainText);
        return new Password(@protected, secureString);
    }

    public static string Unprotect(byte[] @protected)
    {
        byte[] bytes = ProtectedData.Unprotect(@protected, Entropy, DataProtectionScope.CurrentUser);
        string password = Encoding.UTF8.GetString(bytes);
        return password;
    }

    private static byte[] Protect(string password)
    {
        byte[] bytes = !string.IsNullOrEmpty(password)
            ? Encoding.UTF8.GetBytes(password)
            : [];
        byte[] protectedBytes = ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser);
        return protectedBytes;
    }
}