using System.Reflection;

namespace Qurre.Internal.EventsManager
{
    internal struct EventCallMethod
    {
        internal EventCallMethod(MethodInfo info, int priority)
        {
            Info = info;
            Priority = priority;
        }

        internal MethodInfo Info { get; }
        internal int Priority { get; }
    }
}