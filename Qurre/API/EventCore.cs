using System;
using System.Reflection;
using JetBrains.Annotations;
using Qurre.API.Attributes;
using Qurre.Events;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using EventLists = Qurre.Internal.EventsManager.Lists;
using Version = Qurre.API.Addons.Version;

namespace Qurre.API;

[PublicAPI]
public static class EventCore
{
    public static Version Version { get; } = new();

    public static void InjectEventMethod(MethodInfo method)
    {
        if (method.IsAbstract)
            throw new InvalidOperationException($"Cannot inject an abstract method: '{method.Name}'.");

        foreach (var attribute in method.GetCustomAttributes<EventMethod>())
        {
            var callMethod = new EventCallMethod(method, attribute.Priority);
            RegisterEventCall(attribute.Type, callMethod);
        }
    }

    public static void ExtractEventMethod(MethodInfo method)
    {
        if (method.IsAbstract)
            throw new InvalidOperationException($"Cannot inject an abstract method: '{method.Name}'.");

        foreach (var attribute in method.GetCustomAttributes<EventMethod>())
        {
            if (!EventLists.CallMethods.TryGetValue(attribute.Type, out var list))
                continue;

            list.RemoveAll(eventCall => eventCall is EventCallMethod callMethod && callMethod.Info == method);
        }
    }


    public static void InjectAction(uint eventId, int priority, Action<IBaseEvent> action)
    {
        var callAction = new EventCallAction(action, priority);
        RegisterEventCall(eventId, callAction);
    }

    private static void RegisterEventCall(uint eventId, IEventCall eventCall)
    {
        if (!EventLists.CallMethods.TryGetValue(eventId, out var callList))
        {
            callList = [];
            EventLists.CallMethods[eventId] = callList;
        }

        Log.Info("Call Method added " + (eventId == PlayerEvents.Dead));
        callList.Add(eventCall);
    }

    public static void ExtractAction(uint eventId, Action<IBaseEvent> action)
    {
        if (!EventLists.CallMethods.TryGetValue(eventId, out var list))
            return;

        list.RemoveAll(eventCall => eventCall is EventCallAction callAction && callAction.Action == action);
    }


    public static void SortMethodsPriority()
    {
        EventLists.SortAllCallMethodsByPriority();
    }

    public static void SortMethodsPriority(uint eventId)
    {
        EventLists.SortCallMethodsByPriority(eventId);
    }
}