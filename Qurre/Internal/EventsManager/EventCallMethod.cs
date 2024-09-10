using System.Reflection;

namespace Qurre.Internal.EventsManager;

internal readonly struct EventCallMethod(MethodInfo info, int priority)
{
    internal MethodInfo Info { get; } = info;
    internal int Priority { get; } = priority;
}