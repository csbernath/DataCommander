
namespace Foundation.Data.PTypes;

/// <summary>
/// A Microsoft SQL Server stored procedure parameter can be NULL, DEFAULT, too.
/// </summary>
public enum PValueType
{
    Default,
    Empty,
    Null,
    Value
}