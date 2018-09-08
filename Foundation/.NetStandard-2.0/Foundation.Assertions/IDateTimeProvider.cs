using System;

namespace Foundation
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}