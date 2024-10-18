using Qurre.Events.Structs;

namespace Qurre.Internal.EventsManager;

public interface IEventCall
{
    int Priority { get; }
    string Identifier { get; }
    void Call(IBaseEvent @event);
}