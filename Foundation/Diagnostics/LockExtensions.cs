using System;
using System.Threading;

namespace Foundation.Diagnostics;

public static class LockExtensions
{
    extension(Lock @lock)
    {  
        public void TryLock(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (@lock.TryEnter())
            {
                try
                {
                    action();
                }
                finally
                {
                    @lock.Exit();
                }
            }
        }       
    }
}