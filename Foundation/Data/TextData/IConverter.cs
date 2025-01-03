namespace Foundation.Data.TextData;

public interface IConverter<in TInput, out TOutput>
{
    TOutput Convert(TInput input);
}