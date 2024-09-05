using System.Reflection;

namespace Qurre.Internal.EventsManager
{
    internal readonly struct EventCallMethod
    {
        internal MethodInfo Info { get; }
        internal int Priority { get; }

        internal EventCallMethod(MethodInfo info, int priority)
        {
            Info = info;
            Priority = priority;
        }
    }
}