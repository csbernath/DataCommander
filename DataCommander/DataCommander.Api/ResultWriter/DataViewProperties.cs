namespace DataCommander.Api.ResultWriter;

public sealed class DataViewProperties(string? rowFilter, string? sort)
{
    public readonly string? RowFilter = rowFilter;
    public readonly string? Sort = sort;
}