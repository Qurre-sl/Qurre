using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qurre.API;
using Qurre.API.Attributes;
using Qurre.Events.Structs;
using Qurre.Loader;

namespace Qurre.Internal.EventsManager;

internal static class Loader
{
    internal static void PluginPath(Assembly assembly)
    {
        foreach (var method in assembly.GetTypes().Where(x => x.IsClass)
                     .SelectMany(x => x.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                                   BindingFlags.NonPublic)))
        {
            if (method.IsAbstract)
            {
                Log.Debug($"Events Loader: '{method.Name}' is abstract, skip..");
                continue;
            }

            List<EventMethod> attrs = [.. method.GetCustomAttributes<EventMethod>()];

            if (!attrs.Any())
                continue;

            if (method.GetCustomAttribute<EventsIgnore>() is not null)
            {
                Log.Debug($"Events Loader: '{method.Name}' includes [EventsIgnore] attribute, skip..");
                continue;
            }

            foreach (var attributes in attrs)
                if (Lists.CallMethods.TryGetValue(attributes.Type, out var list))
                    list.Add(new EventCallMethod(method, attributes.Priority));
                else Lists.CallMethods.Add(attributes.Type, [new EventCallMethod(method, attributes.Priority)]);
        }
    }

    internal static void InvokeEvent(this IBaseEvent ev)
    {
        if (Lists.QurreMethods.TryGetValue(ev.EventId, out var qurreList))
            foreach (var method in qurreList)
                try
                {
                    if (!method.IsStatic) throw new Exception("Qurre event can not be non-static");

                    if (method.GetParameters().Length == 0) method.Invoke(null, []);
                    else method.Invoke(null, [ev]);
                }
                catch (Exception ex)
                {
                    Log.Error(
                        $"Method '{method.Name}' of class '{method.ReflectedType?.FullName}' threw an exception. Event ID: {ev.EventId}\n{ex}");
                }

        if (!Lists.CallMethods.TryGetValue(ev.EventId, out var list))
            return;

        foreach (var eventCall in list)
            try
            {
                eventCall.Call(ev);
            }
            catch (Exception ex)
            {
                Log.Error($"{eventCall.Identifier} threw an exception. Event ID: {ev.EventId}\n{ex}");
            }
    }
}