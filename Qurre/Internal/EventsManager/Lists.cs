using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qurre.API.Attributes;
using Qurre.Loader;

namespace Qurre.Internal.EventsManager;

internal static class Lists
{
    private static readonly Dictionary<uint, List<MethodInfo>> QurreMethods = [];
    private static readonly Dictionary<uint, List<IEventCall>> CallMethods = [];
    private static readonly Dictionary<MethodInfo, object> ClassesOfNonStaticMethods = [];

    static Lists()
    {
        EntryPoint.Init += PathQurreEvents;
        Plugins.Unloaded += OnPluginsUnloaded;
    }

    private static void OnPluginsUnloaded()
    {
        CallMethods.Clear();
        ClassesOfNonStaticMethods.Clear();
    }

    public static void SortAllCallMethodsByPriority()
    {
        foreach (var eventId in CallMethods.Keys)
            SortCallMethodsByPriority(eventId);
    }
    
    public static void SortCallMethodsByPriority(uint eventId)
    {
        if (!CallMethods.TryGetValue(eventId, out var list))
            return;
        
        list.Sort((eventCallA, eventCallB) => eventCallB.Priority.CompareTo(eventCallA.Priority));
    }
    
    private static void PathQurreEvents()
    {
        foreach (var method in Assembly.GetExecutingAssembly().GetTypes()
                     .Where(type => type.IsClass)
                     .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                     .Where(methodInfo => !methodInfo.IsAbstract))
        {
            foreach (var attribute in method.GetCustomAttributes<EventMethod>())
                if (QurreMethods.TryGetValue(attribute.Type, out var list))
                    list.Add(method);
                else
                    QurreMethods[attribute.Type] = [method];
        }
    }
}