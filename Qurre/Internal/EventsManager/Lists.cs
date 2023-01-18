using System.Collections.Generic;
using System.Reflection;

namespace Qurre.Internal.EventsManager
{
    internal static class Lists
    {
        internal static Dictionary<uint, List<MethodInfo>> QurreMethods = new ();
        internal static Dictionary<uint, List<EventCallMethod>> CallMethods = new ();
        internal static Dictionary<MethodInfo, object> ClassesOfNonStaticMethods = new ();
    }
}