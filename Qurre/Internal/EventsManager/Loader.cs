using Qurre.API;
using Qurre.API.Attributes;
using Qurre.Events.Structs;
using System;
using System.Linq;
using System.Reflection;

namespace Qurre.Internal.EventsManager
{
    static internal class Loader
    {
        static internal void UnloadPlugins()
        {
            List.CallMethods.Clear();
            List.ClassesOfNonStaticMethods.Clear();
            PathQurreEvents();
        }

        static internal void PathQurreEvents()
        {
            foreach (var method in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && x.Namespace == "Qurre.Internal.EventsCalled")
                .SelectMany(x => x.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                if (method.IsAbstract) continue;
                var attr = method.GetCustomAttribute<EventMethod>();
                if (attr is null) continue;
                if (List.CallMethods.TryGetValue(attr.Type, out var list)) list.Add(method);
                else List.CallMethods.Add(attr.Type, new() { method });
            }
        }

        static internal void PluginPath(Assembly assembly)
        {
            foreach (var method in assembly.GetTypes().Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                if (method.IsAbstract)
                {
                    Log.Debug($"Events Loader: '{method.Name}' is abstract, skip..");
                    continue;
                }
                var attr = method.GetCustomAttribute<EventMethod>();
                if (attr is null) continue;
                if (List.CallMethods.TryGetValue(attr.Type, out var list)) list.Add(method);
                else List.CallMethods.Add(attr.Type, new() { method });
            }
        }

        static internal void InvokeEvent(this IBaseEvent @event)
        {
            if (!List.CallMethods.TryGetValue(@event.EventId, out var list)) return;
            foreach (var method in list)
            {
                try
                {
                    if (method.IsStatic) method.Invoke(null, new object[] { @event });
                    else
                    {
                        if (List.ClassesOfNonStaticMethods.TryGetValue(method, out object @class))
                            method.Invoke(@class, new object[] { @event });
                        else
                        {
                            Type type = method.DeclaringType;
                            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                            @class = constructor.Invoke(new object[] { });
                            List.ClassesOfNonStaticMethods.Add(method, @class);
                            method.Invoke(@class, new object[] { @event });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Method '{method.Name}' of class {method.ReflectedType?.FullName} threw an exception. Event ID: {@event.EventId}\n{ex}");
                }
            }
        }
    }
}