using System.Collections.Generic;
using System.Reflection;
using Qurre.API.Attributes;

namespace Qurre.Loader
{
    internal struct PluginStruct
    {
        internal readonly List<MethodStruct> EnableMethods = new ();
        internal readonly List<MethodStruct> DisableMethods = new ();
        internal readonly PluginInit Info;

        internal PluginStruct(PluginInit info) => Info = info;
    }

    internal struct MethodStruct
    {
        internal readonly MethodInfo MethodInfo;
        internal readonly PluginInit Info;
        internal readonly int Priority;

        internal MethodStruct(MethodInfo method, PluginInit info, int priority)
        {
            MethodInfo = method;
            Info = info;
            Priority = priority;
        }
    }
}