namespace Foundation.Core;

public struct DateInterval(Date start, Date end)
{
    public readonly Date Start = start;
    public readonly Date End = end;
}