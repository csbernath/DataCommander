namespace DataCommander.Api;

public class FilterCriterion
{
    public readonly string Property;
    public readonly string Value;

    public FilterCriterion(string property, string value)
    {
        Property = property;
        Value = value;
    }
}