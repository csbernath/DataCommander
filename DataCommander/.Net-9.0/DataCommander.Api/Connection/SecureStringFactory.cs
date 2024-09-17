using System.Security;

namespace DataCommander.Api.Connection;

internal static class SecureStringFactory
{
    public static SecureString CreateFromPlainText(string password)
    {
        SecureString secureString = new SecureString();
        foreach (char character in password)
            secureString.AppendChar(character);
        secureString.MakeReadOnly();
        return secureString;
    }
}