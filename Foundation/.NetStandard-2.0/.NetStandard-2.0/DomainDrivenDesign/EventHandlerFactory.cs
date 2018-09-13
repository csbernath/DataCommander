using System;

namespace Foundation.DomainDrivenDesign
{
    public static class EventHandlerFactory
    {
        public static IAsyncEventHandler<TEvent> Create<TEvent>(Action<TEvent> handle) where TEvent : IEvent
        {
            return new EventHandler<TEvent>(handle);
        }

        public static IAsyncEventHandler<TEvent> Create<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            return new EventHandler<TEvent>(handler.Handle);
        }
    }
}