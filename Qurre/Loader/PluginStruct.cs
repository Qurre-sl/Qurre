using System.Collections.Generic;
using System.Reflection;
using Qurre.API.Attributes;

namespace Qurre.Loader;

internal readonly struct PluginStruct(PluginInit info)
{
    internal List<MethodStruct> EnableMethods { get; } = [];
    internal List<MethodStruct> DisableMethods { get; } = [];
    internal PluginInit Info { get; } = info;
}

internal readonly struct MethodStruct(
    MethodInfo method,
    PluginInit info,
    int priority
)
{
    internal MethodInfo MethodInfo { get; } = method;
    internal PluginInit Info { get; } = info;
    internal int Priority { get; } = priority;
}