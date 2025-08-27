using System;

namespace Foundation.Core;

public static class IDateTimeProviderExtensions
{
    extension(IDateTimeProvider dateTimeProvider)
    {
        public DateTime Today
        {
            get
            {
                ArgumentNullException.ThrowIfNull(dateTimeProvider);
                return dateTimeProvider.Now.Date;
            }
        }
    }
}