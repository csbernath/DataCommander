using System.Security;

namespace DataCommander.Api.Connection;

public sealed class Password(byte[] @protected, SecureString secureString)
{
    public readonly byte[] Protected = @protected;
    public readonly SecureString SecureString = secureString;
}