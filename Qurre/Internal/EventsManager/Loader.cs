using Qurre.API.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.Internal.EventsManager
{
    static internal class Loader
    {
        static internal void PluginPath(Assembly assembly)
        {
            foreach (var method in assembly.GetTypes().Where(x => x.IsClass).SelectMany(x => x.GetMethods()))
            {
                var attr = method.GetCustomAttribute<EventMethod>();
                if (attr is null) continue;
                if (List.CallMethods.TryGetValue(attr.Type, out var list)) list.Add(method);
                else List.CallMethods.Add(attr.Type, new() { method });
            }
        }
    }
}