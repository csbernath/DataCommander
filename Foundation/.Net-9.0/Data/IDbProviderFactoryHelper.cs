namespace Foundation.Data;

public interface IDbProviderFactoryHelper
{
    IDbCommandHelper DbCommandHelper { get; }

    IDbCommandBuilderHelper DbCommandBuilderHelper { get; }
}