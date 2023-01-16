using HarmonyLib;
using Qurre.API;
using Qurre.API.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Qurre.Loader
{
    static internal class Plugins
    {
        static internal readonly List<PluginStruct> _plugins = new();
        static internal Harmony _harmony;

        static internal void Init()
        {
            if (!Directory.Exists(Pathes.Plugins))
            {
                Log.Warn($"Plugins directory not found. Creating: {Pathes.Plugins}");
                Directory.CreateDirectory(Pathes.Plugins);
            }

            try { LoadDependencies(); }
            catch (Exception ex) { ServerConsole.AddLog(ex.ToString(), ConsoleColor.Red); }

            PatchMethods();

            LoadPlugins();

            Internal.EventsManager.Loader.SortMethods();

            MEC.Timing.RunCoroutine(EnablePluginsInThread());

            static IEnumerator<float> EnablePluginsInThread()
            {
                EnablePlugins();
                yield break;
            }
        }

        static void LoadDependencies()
        {
            if (!Directory.Exists(Pathes.Dependencies))
                Directory.CreateDirectory(Pathes.Dependencies);

            foreach (string dll in Directory.GetFiles(Pathes.Dependencies))
            {
                if (!dll.EndsWith(".dll") || LoaderManager.Loaded(dll)) continue;

                Assembly assembly = Assembly.Load(LoaderManager.ReadFile(dll));
                LoaderManager.LocalLoaded.Add(new(assembly, dll));

                Log.Custom("Loaded dependency " + assembly.FullName, "Loader", ConsoleColor.Blue);
            }

            Log.Custom("Dependencies loaded", "Loader", ConsoleColor.Green);
        }

        static void PatchMethods()
        {
            try
            {
                _harmony = new Harmony("qurre.patches");
                _harmony.PatchAll();

                Log.Info("Harmony successfully Patched");
            }
            catch (Exception ex)
            {
                Log.Error($"Harmony Patching threw an error:\n{ex}");
            }
        }

        static void LoadPlugins()
        {
            foreach (string plugin in Directory.GetFiles(Pathes.Plugins))
            {
                try
                {
                    Log.Debug($"Loading {plugin}");
                    var assembly = Assembly.Load(LoaderManager.ReadFile(plugin));
                    bool loaded = LoadPlugin(assembly);
                    if (loaded) Internal.EventsManager.Loader.PluginPath(assembly);
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occurred while loading {plugin}\n{ex}");
                }
            }
        }

        static bool LoadPlugin(Assembly assembly)
        {
            try
            {
                bool loaded = false;
                foreach (Type type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<PluginInit>();
                    if (attr is null) continue;

                    Log.Debug($"Loading plugin {attr.Name} ({type.FullName})");

                    loaded = true;
                    var instance = Activator.CreateInstance(type);

                    PluginStruct plugin = new(attr);

                    foreach (var methodInfo in instance.GetType()
                        .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (methodInfo.IsAbstract)
                        {
                            Log.Debug($"Plugin Loader: '{methodInfo.Name}' is abstract, skip..");
                            continue;
                        }
                        if (!methodInfo.IsStatic)
                        {
                            Log.Debug($"Plugin Loader: '{methodInfo.Name}' is non-static, skip..");
                            continue;
                        }

                        if (methodInfo.GetCustomAttribute<PluginEnable>() is not null)
                            plugin.EnableMethods.Add(new(methodInfo, attr, getPriority(methodInfo)));

                        if (methodInfo.GetCustomAttribute<PluginDisable>() is not null)
                            plugin.DisableMethods.Add(new(methodInfo, attr, getPriority(methodInfo)));
                    }

                    _plugins.Add(plugin);

                    Log.Debug($"{type.FullName} loaded");

                    Log.Custom($"Plugin {plugin.Info.Name} written by {plugin.Info.Developer} loaded. v{plugin.Info.Version}", "Loader", ConsoleColor.Magenta);
                }

                if (!loaded) Log.Debug($"{assembly.FullName} doesn't have a class with [PluginInit] attribute");

                return loaded;
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while loading {assembly.FullName}\n{ex}");
            }

            return false;

            static int getPriority(MethodInfo method)
            {
                var attr = method.GetCustomAttribute<PluginPriority>();
                return attr is null ? 0 : attr.Priority;
            }
        }

        static void EnablePlugins()
        {
            foreach (MethodStruct method in _plugins.SelectMany(x => x.EnableMethods).OrderByDescending(x => x.Priority))
            {
                try
                {
                    method.MethodInfo.Invoke(null, new object[] { });
                    Log.Info($"Plugin {method.Info.Name} [{method.MethodInfo.Name}] written by {method.Info.Developer} enabled. v{method.Info.Version}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Plugin {method.Info.Name} [{method.MethodInfo.Name}] threw an exception while enabling\n{ex}");
                }
            }
        }

        static void Disable()
        {
            foreach (MethodStruct method in _plugins.SelectMany(x => x.EnableMethods))
            {
                try
                {
                    method.MethodInfo.Invoke(null, new object[] { });
                    Log.Info($"Plugin {method.Info.Name} [{method.MethodInfo.Name}] disabled");
                }
                catch (Exception ex)
                {
                    Log.Error($"Plugin {method.Info.Name} [{method.MethodInfo.Name}] threw an exception while disabling\n{ex}");
                }
            }
        }

        static internal void ReloadPlugins()
        {
            try
            {
                Log.Info("Plugins are reloading...");

                Disable();
                Internal.EventsManager.Loader.UnloadPlugins();
                _plugins.Clear();

                LoadPlugins();
                EnablePlugins();
                Internal.EventsManager.Loader.SortMethods();

                Log.Info("Plugins reloaded");
            }
            catch (Exception ex)
            {
                Log.Error($"Reload Plugins error..\n{ex}");
            }
        }
    }
}