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
            Lists.CallMethods.Clear();
            Lists.ClassesOfNonStaticMethods.Clear();
        }

        static internal void SortMethods() // need to optimize
        {
            foreach (var item in Lists.CallMethods)
            {
                var onetime = item.Value.ToList().OrderByDescending(x => x.Priority);
                item.Value.Clear();
                item.Value.InsertRange(0, onetime);
            }
        }

        static internal void PathQurreEvents()
        {
            foreach (var method in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && x.Namespace == "Qurre.Internal.EventsCalled")
                .SelectMany(x => x.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                if (method.IsAbstract) continue;
                var attrs = method.GetCustomAttributes<EventMethod>();
                if (attrs is null) continue;
                foreach (var attr in attrs)
                {
                    if (Lists.QurreMethods.TryGetValue(attr.Type, out var list)) list.Add(method);
                    else Lists.QurreMethods.Add(attr.Type, new() { method });
                }
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

                var attrs = method.GetCustomAttributes<EventMethod>();

                if (attrs is null || attrs.Count() == 0)
                    continue;

                if (method.GetCustomAttribute<EventsIgnore>() is not null)
                {
                    Log.Debug($"Events Loader: '{method.Name}' includes [EventsIgnore] attribute, skip..");
                    continue;
                }

                foreach (var attr in attrs)
                {
                    if (Lists.CallMethods.TryGetValue(attr.Type, out var list)) list.Add(new(method, attr.Priority));
                    else Lists.CallMethods.Add(attr.Type, new() { new(method, attr.Priority) });
                }
            }
        }

        static internal void InvokeEvent(this IBaseEvent @event)
        {
            if (Lists.QurreMethods.TryGetValue(@event.EventId, out var qurreList))
            {
                foreach (var method in qurreList)
                {
                    try
                    {
                        if (!method.IsStatic)
                            throw new Exception("Qurre event can not be non-static");
                        else
                        {
                            if (method.GetParameters().Length == 0) method.Invoke(null, new object[] { });
                            else method.Invoke(null, new object[] { @event });
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Method '{method.Name}' of class {method.ReflectedType?.FullName} threw an exception. Event ID: {@event.EventId}\n{ex}");
                    }
                }
            }

            if (!Lists.CallMethods.TryGetValue(@event.EventId, out var list))
                return;

            foreach (var metStruct in list)
            {
                var method = metStruct.Info;
                try
                {
                    if (method.IsStatic)
                        Invoke(null);
                    else
                    {
                        if (Lists.ClassesOfNonStaticMethods.TryGetValue(method, out object @class))
                            Invoke(@class);
                        else
                        {
                            Type type = method.DeclaringType;
                            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                            @class = constructor.Invoke(new object[] { });
                            Lists.ClassesOfNonStaticMethods.Add(method, @class);
                            Invoke(@class);
                        }
                    }

                    void Invoke(object @class)
                    {
                        if (method.GetParameters().Length == 0) method.Invoke(@class, new object[] { });
                        else method.Invoke(@class, new object[] { @event });
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