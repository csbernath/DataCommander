using System;

namespace DataCommander.Update.Events
{
    public sealed class ExceptionOccured : Event
    {
        public readonly Exception Exception;

        public ExceptionOccured(Exception exception)
        {
            Exception = exception;
        }
    }
}