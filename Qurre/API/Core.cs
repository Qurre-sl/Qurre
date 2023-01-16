using Qurre.API.Attributes;
using System;
using System.Linq;
using System.Reflection;
using EventLists = Qurre.Internal.EventsManager.Lists;

namespace Qurre.API
{
    static public class Core
    {
        static public Version Version { get; private set; } = new(2, 0);

        static public void InjectEventMethod(MethodInfo method)
        {
            if (method.IsAbstract)
                throw new Exception($"InjectEventMethod: '{method.Name}' is abstract");

            var attrs = method.GetCustomAttributes<EventMethod>();
            if (attrs is null)
                return;

            foreach (var attr in attrs)
            {
                if (EventLists.CallMethods.TryGetValue(attr.Type, out var list)) list.Add(new(method, attr.Priority));
                else EventLists.CallMethods.Add(attr.Type, new() { new(method, attr.Priority) });
            }
        }

        static public void SortMethodsPriority()
            => Internal.EventsManager.Loader.SortMethods();
        static public void SortMethodsPriority(uint eventId)
        {
            if (!EventLists.CallMethods.TryGetValue(eventId, out var list))
                return;

            var onetime = list.OrderByDescending(x => x.Priority);
            list.Clear();
            list.AddRange(onetime);
        }
    }
}