using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace DataCommander.Api.Connection;

public static class PasswordFactory
{
    private static readonly byte[] Entropy = [0x56, 0x4f, 0x3d, 0x78, 0xf1];

    [SupportedOSPlatform("windows")]
    public static Password CreateFromPlainText(string plainText)
    {
        var @protected = Protect(plainText);
        var secureString = SecureStringFactory.CreateFromPlainText(plainText);
        return new Password(@protected, secureString);
    }

    [SupportedOSPlatform("windows")]
    public static Password CreateFromProtected(byte[] @protected)
    {
        var plainText = Unprotect(@protected);
        var secureString = SecureStringFactory.CreateFromPlainText(plainText);
        return new Password(@protected, secureString);
    }

    [SupportedOSPlatform("windows")]
    public static string Unprotect(byte[] @protected)
    {
        var bytes = ProtectedData.Unprotect(@protected, Entropy, DataProtectionScope.CurrentUser);
        var password = Encoding.UTF8.GetString(bytes);
        return password;
    }

    [SupportedOSPlatform("windows")]
    private static byte[] Protect(string password)
    {
        var bytes = !string.IsNullOrEmpty(password)
            ? Encoding.UTF8.GetBytes(password)
            : [];
        var protectedBytes = ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser);
        return protectedBytes;
    }
}