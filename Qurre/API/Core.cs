namespace Qurre.API;

using Qurre.API.Addons;
using Qurre.API.Attributes;
using System.Linq;
using System.Reflection;
using EventLists = Internal.EventsManager.Lists;

static public class Core
{
    static public Version Version { get; } = new();

    static public void InjectEventMethod(MethodInfo method)
    {
        if (method.IsAbstract)
            throw new System.Exception($"InjectEventMethod: '{method.Name}' is abstract");

        var attrs = method.GetCustomAttributes<EventMethod>();
        if (attrs is null)
            return;

        foreach (var attr in attrs)
        {
            if (EventLists.CallMethods.TryGetValue(attr.Type, out var list))
                list.Add(new(method, attr.Priority));
            else
                EventLists.CallMethods.Add(attr.Type, new() { new(method, attr.Priority) });
        }
    }

    static public void SortMethodsPriority()
        => Internal.EventsManager.Loader.SortMethods();
    static public void SortMethodsPriority(uint eventId)
    {
        if (!EventLists.CallMethods.TryGetValue(eventId, out var list))
            return;

        var onetime = list.ToList().OrderByDescending(x => x.Priority);
        list.Clear();
        list.InsertRange(0, onetime);
    }
}