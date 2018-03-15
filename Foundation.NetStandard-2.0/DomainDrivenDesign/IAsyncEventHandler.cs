namespace Foundation.DomainDrivenDesign
{
    public interface IAsyncEventHandler<in TEvent> : IOneWayMessageHandler<TEvent> where TEvent : IEvent
    {
    }
}