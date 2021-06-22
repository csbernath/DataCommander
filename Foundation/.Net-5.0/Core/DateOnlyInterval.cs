namespace Foundation.Core
{
    public readonly struct DateOnlyInterval
    {
        public readonly DateOnly Start;
        public readonly DateOnly End;

        public DateOnlyInterval(DateOnly start, DateOnly end)
        {
            Start = start;
            End = end;
        }
    }
}