﻿namespace Foundation.Core;

public readonly struct DateInterval(Date start, Date end)
{
    public readonly Date Start = start;
    public readonly Date End = end;
}