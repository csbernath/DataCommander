using System;

namespace Foundation.Core
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}