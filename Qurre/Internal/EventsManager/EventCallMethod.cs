using System;
using System.Reflection;
using Qurre.Events.Structs;

namespace Qurre.Internal.EventsManager;

internal class EventCallMethod : IEventCall
{
    internal EventCallMethod(MethodInfo info, int priority)
    {
        Info = info;
        Priority = priority;
        Identifier = $"Method '{Info.Name}' of class '{Info.ReflectedType?.FullName}'";
    }

    internal MethodInfo Info { get; }
    public int Priority { get; }
    public string Identifier { get; }

    public void Call(IBaseEvent @event)
    {
        if (Info.IsStatic)
        {
            Invoke(@event, null);
            return;
        }

        if (Lists.ClassesOfNonStaticMethods.TryGetValue(Info, out object @class))
        {
            Invoke(@event, @class);
            return;
        }

        Type? type = Info.DeclaringType;
        ConstructorInfo? constructor = type?.GetConstructor(Type.EmptyTypes);

        @class = constructor?.Invoke([]) ?? throw new NullReferenceException(nameof(constructor));
        Lists.ClassesOfNonStaticMethods.Add(Info, @class);

        Invoke(@event, @class);
    }

    private void Invoke(IBaseEvent @event, object? root)
    {
        if (Info.GetParameters().Length == 0) Info.Invoke(root, []);
        else Info.Invoke(root, [@event]);
    }
}