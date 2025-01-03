using System;
using Foundation.Assertions;

namespace Foundation.Core;

public static class IDateTimeProviderExtensions
{
    public static DateTime Today(this IDateTimeProvider dateTimeProvider)
    {
        Assert.IsNotNull(dateTimeProvider, nameof(dateTimeProvider));
        return dateTimeProvider.Now.Date;
    }
}