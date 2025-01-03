namespace DataCommander.Application.Connection;

public class CredentialDto(string userId, byte[] password)
{
    public readonly string UserId = userId;
    public readonly byte[] Password = password;
}