using System.Reflection;

namespace Qurre.Internal.EventsManager
{
    internal struct EventCallMethod
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