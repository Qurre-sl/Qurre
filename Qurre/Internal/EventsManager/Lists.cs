using System.Collections.Generic;
using System.Reflection;

namespace Qurre.Internal.EventsManager;

internal static class Lists
{
    internal static readonly Dictionary<uint, List<MethodInfo>> QurreMethods = [];
    internal static readonly Dictionary<uint, List<IEventCall>> CallMethods = [];
    internal static readonly Dictionary<MethodInfo, object> ClassesOfNonStaticMethods = [];
}