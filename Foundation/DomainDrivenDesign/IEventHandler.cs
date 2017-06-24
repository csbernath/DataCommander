using System;

namespace Foundation.DomainDrivenDesign
{
    public interface IEventHandler<in TEvent>
    {
        void Handle(TEvent @event);
    }
}