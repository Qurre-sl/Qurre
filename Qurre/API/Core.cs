using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qurre.API.Attributes;
using Qurre.Internal.EventsManager;
using EventLists = Qurre.Internal.EventsManager.Lists;

namespace Qurre.API
{
    public static class Core
    {
        public static Version Version { get; } = new (2, 0);

        public static void InjectEventMethod(MethodInfo method)
        {
            if (method.IsAbstract)
            {
                throw new ($"InjectEventMethod: '{method.Name}' is abstract");
            }

            IEnumerable<EventMethod> attrs = method.GetCustomAttributes<EventMethod>();

            if (attrs is null)
            {
                return;
            }

            foreach (EventMethod attr in attrs)
            {
                if (EventLists.CallMethods.TryGetValue(attr.Type, out List<EventCallMethod> list))
                {
                    list.Add(new (method, attr.Priority));
                }
                else
                {
                    EventLists.CallMethods.Add(attr.Type, new () { new (method, attr.Priority) });
                }
            }
        }

        public static void SortMethodsPriority()
            => Internal.EventsManager.Loader.SortMethods();

        public static void SortMethodsPriority(uint eventId)
        {
            if (!EventLists.CallMethods.TryGetValue(eventId, out List<EventCallMethod> list))
            {
                return;
            }

            IOrderedEnumerable<EventCallMethod> onetime = list.OrderByDescending(x => x.Priority);
            list.Clear();
            list.AddRange(onetime);
        }
    }
}