using System;
using System.Threading.Tasks;

namespace Foundation.DomainDrivenDesign
{
    internal sealed class EventHandler<TEvent> : IAsyncEventHandler<TEvent> where TEvent : IEvent
    {
        private readonly Action<TEvent> _handle;

        public EventHandler(Action<TEvent> handle)
        {
            _handle = handle;
        }

        public async Task<VoidResponse> HandleAsync(TEvent @event)
        {
            await Task.Run(() => _handle(@event));
            return VoidResponse.Default;
        }
    }
}