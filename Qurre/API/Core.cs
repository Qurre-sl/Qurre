using System;
using System.Reflection;
using JetBrains.Annotations;
using Qurre.API.Attributes;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using EventLists = Qurre.Internal.EventsManager.Lists;
using Version = Qurre.API.Addons.Version;

namespace Qurre.API;

[PublicAPI]
public static class Core
{
    public static Version Version { get; } = new();

    public static void InjectEventMethod(MethodInfo method)
    {
        if (method.IsAbstract)
            throw new Exception($"InjectEventMethod: '{method.Name}' is abstract");

        var attrs = method.GetCustomAttributes<EventMethod>();

        foreach (EventMethod? attr in attrs)
            if (EventLists.CallMethods.TryGetValue(attr.Type, out var list))
                list.Add(new EventCallMethod(method, attr.Priority));
            else
                EventLists.CallMethods.Add(attr.Type, [new EventCallMethod(method, attr.Priority)]);
    }

    public static void ExtractEventMethod(MethodInfo method)
    {
        if (method.IsAbstract)
            throw new Exception($"InjectEventMethod: '{method.Name}' is abstract");

        var attrs = method.GetCustomAttributes<EventMethod>();

        foreach (EventMethod? attr in attrs)
        {
            if (!EventLists.CallMethods.TryGetValue(attr.Type, out var list))
                continue;

            list.RemoveAll(x => x is EventCallMethod call && call.Info == method);
        }
    }


    public static void InjectAction(uint eventId, int priority, Action<IBaseEvent> action)
    {
        if (EventLists.CallMethods.TryGetValue(eventId, out var list))
            list.Add(new EventCallAction(action, priority));
        else
            EventLists.CallMethods.Add(eventId, [new EventCallAction(action, priority)]);
    }

    public static void ExtractAction(uint eventId, Action<IBaseEvent> action)
    {
        if (!EventLists.CallMethods.TryGetValue(eventId, out var list))
            return;

        list.RemoveAll(x => x is EventCallAction call && call.Action == action);
    }


    public static void SortMethodsPriority()
    {
        Internal.EventsManager.Loader.SortMethods();
    }

    public static void SortMethodsPriority(uint eventId)
    {
        if (!EventLists.CallMethods.TryGetValue(eventId, out var list))
            return;

        list.Sort((x, y) => y.Priority.CompareTo(x.Priority));
    }
}