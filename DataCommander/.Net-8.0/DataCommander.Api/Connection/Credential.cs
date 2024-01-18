namespace DataCommander.Api.Connection;

public sealed class Credential(string userId, Password password)
{
    public readonly string UserId = userId;
    public readonly Password Password = password;
}