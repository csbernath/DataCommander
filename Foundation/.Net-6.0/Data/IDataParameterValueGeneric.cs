
namespace Foundation.Data;

public interface IDataParameterValue<out T> : IDataParameterValue
{
    T Value { get; }
}