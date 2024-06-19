using System.Security;

namespace DataCommander.Api.Connection;

internal static class SecureStringFactory
{
    public static SecureString CreateFromPlainText(string password)
    {
        var secureString = new SecureString();
        foreach (var character in password)
            secureString.AppendChar(character);
        secureString.MakeReadOnly();
        return secureString;
    }
}