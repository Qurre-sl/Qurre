using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qurre.API;
using Qurre.API.Attributes;
using Qurre.Events.Structs;

namespace Qurre.Internal.EventsManager
{
    internal static class Loader
    {
        internal static void UnloadPlugins()
        {
            Lists.CallMethods.Clear();
            Lists.ClassesOfNonStaticMethods.Clear();
        }

        internal static void SortMethods() // need to optimize
        {
            foreach (KeyValuePair<uint, List<EventCallMethod>> item in Lists.CallMethods)
            {
                IOrderedEnumerable<EventCallMethod> onetime = item.Value.OrderByDescending(x => x.Priority);
                item.Value.Clear();
                item.Value.AddRange(onetime);
            }
        }

        internal static void PathQurreEvents()
        {
            foreach (MethodInfo method in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && x.Namespace == "Qurre.Internal.EventsCalled")
                         .SelectMany(x => x.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                if (method.IsAbstract)
                {
                    continue;
                }

                IEnumerable<EventMethod> attrs = method.GetCustomAttributes<EventMethod>();

                if (attrs is null)
                {
                    continue;
                }

                foreach (EventMethod attr in attrs)
                {
                    if (Lists.QurreMethods.TryGetValue(attr.Type, out List<MethodInfo> list))
                    {
                        list.Add(method);
                    }
                    else
                    {
                        Lists.QurreMethods.Add(attr.Type, new () { method });
                    }
                }
            }
        }

        internal static void PluginPath(Assembly assembly)
        {
            foreach (MethodInfo method in assembly.GetTypes().Where(x => x.IsClass)
                         .SelectMany(x => x.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                if (method.IsAbstract)
                {
                    Log.Debug($"Events Loader: '{method.Name}' is abstract, skip..");
                    continue;
                }

                IEnumerable<EventMethod> attrs = method.GetCustomAttributes<EventMethod>();

                if (attrs is null || attrs.Count() == 0)
                {
                    continue;
                }

                if (method.GetCustomAttributes<EventsIgnore>() is not null)
                {
                    Log.Debug($"Events Loader: '{method.Name}' includes [EventsIgnore] attribute, skip..");
                    continue;
                }

                foreach (EventMethod attr in attrs)
                {
                    if (Lists.CallMethods.TryGetValue(attr.Type, out List<EventCallMethod> list))
                    {
                        list.Add(new (method, attr.Priority));
                    }
                    else
                    {
                        Lists.CallMethods.Add(attr.Type, new () { new (method, attr.Priority) });
                    }
                }
            }
        }

        internal static void InvokeEvent(this IBaseEvent @event)
        {
            if (Lists.QurreMethods.TryGetValue(@event.EventId, out List<MethodInfo> qurreList))
            {
                foreach (MethodInfo method in qurreList)
                {
                    try
                    {
                        if (!method.IsStatic)
                        {
                            throw new ("Qurre event can not be non-static");
                        }

                        if (method.GetParameters().Length == 0)
                        {
                            method.Invoke(null, new object[] { });
                        }
                        else
                        {
                            method.Invoke(null, new object[] { @event });
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Method '{method.Name}' of class {method.ReflectedType?.FullName} threw an exception. Event ID: {@event.EventId}\n{ex}");
                    }
                }
            }

            if (!Lists.CallMethods.TryGetValue(@event.EventId, out List<EventCallMethod> list))
            {
                return;
            }

            foreach (EventCallMethod metStruct in list)
            {
                MethodInfo method = metStruct.Info;

                try
                {
                    if (method.IsStatic)
                    {
                        Invoke(null);
                    }
                    else
                    {
                        if (Lists.ClassesOfNonStaticMethods.TryGetValue(method, out object @class))
                        {
                            Invoke(@class);
                        }
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
                        if (method.GetParameters().Length == 0)
                        {
                            method.Invoke(@class, new object[] { });
                        }
                        else
                        {
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