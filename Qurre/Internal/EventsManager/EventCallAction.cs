using System;
using Qurre.Events.Structs;

namespace Qurre.Internal.EventsManager;

internal class EventCallAction : IEventCall
{
    internal EventCallAction(Action<IBaseEvent> action, int priority)
    {
        Action = action;
        Priority = priority;
        Identifier =
            $"Anonymous action of method '{action.Method.Name}' of class '{action.Method.ReflectedType?.FullName}'";
    }

    internal Action<IBaseEvent> Action { get; }
    public int Priority { get; }
    public string Identifier { get; }

    public void Call(IBaseEvent @event)
    {
        Action.Invoke(@event);
    }
}