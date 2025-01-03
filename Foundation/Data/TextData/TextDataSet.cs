using System.Diagnostics;

namespace Foundation.Data.TextData;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="name"></param>
[DebuggerDisplay("Name = {" + nameof(Name) + "}")]
public sealed class TextDataSet(string name)
{

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// 
    /// </summary>
    public TextDataSetTableCollection Tables { get; } = [];
}