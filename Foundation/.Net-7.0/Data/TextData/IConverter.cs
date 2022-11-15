
namespace Foundation.Data.TextData;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public interface IConverter<in TInput, out TOutput>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    TOutput Convert(TInput input);
}