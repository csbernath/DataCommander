using System;

namespace Foundation.Data.TextData;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="column"></param>
/// <param name="converter"></param>
/// <param name="fieldValue"></param>
/// <param name="innerException"></param>
public class TextDataFormatException(TextDataColumn column, ITextDataConverter converter, string fieldValue, Exception innerException) : FormatException(null, innerException)
{

    /// <summary>
    /// 
    /// </summary>
    public override string Message { get; } = $"Conversion ({converter}) of string value '{fieldValue}' to type {column.DataType} failed. ";

    /// <summary>
    /// 
    /// </summary>
    public TextDataColumn Column { get; } = column;

    /// <summary>
    /// 
    /// </summary>
    public ITextDataConverter Converter { get; } = converter;

    /// <summary>
    /// 
    /// </summary>
    public string Fieldvalue { get; } = fieldValue;
}