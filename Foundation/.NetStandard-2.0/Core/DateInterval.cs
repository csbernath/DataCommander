namespace Foundation.Core
{
    public struct DateInterval
    {
        public readonly Date Start;
        public readonly Date End;

        public DateInterval(Date start, Date end)
        {
            Start = start;
            End = end;
        }
    }
}