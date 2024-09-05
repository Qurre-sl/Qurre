using System.Collections.Generic;
using System.Reflection;

namespace Qurre.Internal.EventsManager
{
    static internal class Lists
    {
        static internal Dictionary<uint, List<MethodInfo>> QurreMethods = new();
        static internal Dictionary<uint, List<EventCallMethod>> CallMethods = new();
        static internal Dictionary<MethodInfo, object> ClassesOfNonStaticMethods = new();
    }
}