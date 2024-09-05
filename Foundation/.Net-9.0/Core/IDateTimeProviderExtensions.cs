using System;

namespace Foundation.Core;

public static class IDateTimeProviderExtensions
{
    public static DateTime Today(this IDateTimeProvider dateTimeProvider)
    {
        ArgumentNullException.ThrowIfNull(dateTimeProvider, nameof(dateTimeProvider));
        return dateTimeProvider.Now.Date;
    }
}