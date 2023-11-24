
namespace Foundation.Data;

/// <summary>
/// 
/// </summary>
public interface IDbProviderFactoryHelper
{
    /// <summary>
    /// 
    /// </summary>
    IDbCommandHelper DbCommandHelper { get; }

    /// <summary>
    /// 
    /// </summary>
    IDbCommandBuilderHelper DbCommandBuilderHelper { get; }
}