using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MEC;
using Qurre.API;
using Qurre.API.Addons;
using Qurre.API.Attributes;

namespace Qurre.Loader;

internal static class Plugins
{
    private static readonly List<PluginStruct> PluginsList = [];
    private static Harmony? _harmony;

    internal static void Init()
    {
        if (!Directory.Exists(Paths.Plugins))
        {
            Log.Warn($"Plugins directory not found. Creating: {Paths.Plugins}");
            Directory.CreateDirectory(Paths.Plugins);
        }

        try
        {
            LoadDependencies();
        }
        catch (Exception ex)
        {
            ServerConsole.AddLog(ex.ToString(), ConsoleColor.Red);
        }

        PatchMethods();

        LoadPlugins();

        Internal.EventsManager.Loader.SortMethods();

        Timing.RunCoroutine(EnablePluginsInThread());
        return;

        static IEnumerator<float> EnablePluginsInThread()
        {
            EnablePlugins();
            yield break;
        }
    }

    private static void LoadDependencies()
    {
        if (!Directory.Exists(Paths.Dependencies))
            Directory.CreateDirectory(Paths.Dependencies);

        foreach (string dll in Directory.GetFiles(Paths.Dependencies))
        {
            if (!dll.EndsWith(".dll") || LoaderManager.Loaded(dll)) continue;

            Assembly assembly = Assembly.Load(LoaderManager.ReadFile(dll));
            LoaderManager.LocalLoaded.Add(new AssemblyDep(assembly, dll));

            Log.Custom("Loaded dependency " + assembly.FullName, "Loader", ConsoleColor.Blue);
        }

        Log.Custom("Dependencies loaded", "Loader", ConsoleColor.Green);
    }

    private static void PatchMethods()
    {
        try
        {
            bool errored = false;
            _harmony = new Harmony("qurre.patches");

            Type? reflectedType = new StackTrace().GetFrame(1)?.GetMethod()?.ReflectedType;
            if (reflectedType is null)
            {
                Log.Error("Harmony Patching threw an error:\nReflectedType is null");
                return;
            }

            Assembly assembly = reflectedType.Assembly;
            AccessTools.GetTypesFromAssembly(assembly).Do(delegate(Type type)
            {
                try
                {
                    _harmony.CreateClassProcessor(type).Patch();
                }
                catch (Exception e)
                {
                    Log.Error($"Excepted error in type: {BetterColors.Yellow(type)}\n{BetterColors.Grey(e)}");
                    errored = true;
                }
            });

            if (!errored)
                Log.Info("Harmony successfully Patched");
        }
        catch (Exception ex)
        {
            Log.Error($"Harmony Patching threw an error:\n{ex}");
        }
    }

    private static void LoadPlugins()
    {
        foreach (string plugin in Directory.GetFiles(Paths.Plugins))
            try
            {
                Log.Debug($"Loading {plugin}");
                Assembly assembly = Assembly.Load(LoaderManager.ReadFile(plugin));
                bool loaded = LoadPlugin(assembly);
                if (loaded) Internal.EventsManager.Loader.PluginPath(assembly);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while loading {plugin}\n{ex}");
            }
    }

    private static bool LoadPlugin(Assembly assembly)
    {
        try
        {
            bool loaded = false;
            foreach (Type type in assembly.GetTypes())
            {
                PluginInit? attr = type.GetCustomAttribute<PluginInit>();
                if (attr is null)
                    continue;

                Log.Debug($"Loading plugin {attr.Name} ({type.FullName})");

                loaded = true;

                PluginStruct plugin = new(attr);

                foreach (MethodInfo methodInfo in type
                             .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                         BindingFlags.NonPublic))
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
                        plugin.EnableMethods.Add(new MethodStruct(methodInfo, attr, GetPriority(methodInfo)));

                    if (methodInfo.GetCustomAttribute<PluginDisable>() is not null)
                        plugin.DisableMethods.Add(new MethodStruct(methodInfo, attr, GetPriority(methodInfo)));
                }

                PluginsList.Add(plugin);

                Log.Debug($"{type.FullName} loaded");

                Log.Custom(
                    $"Plugin {plugin.Info.Name} written by {plugin.Info.Developer} loaded. v{plugin.Info.Version}",
                    "Loader", ConsoleColor.Magenta);
            }

            if (!loaded)
                Log.Debug($"{assembly.FullName} doesn't have a class with [PluginInit] attribute");

            return loaded;
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while loading {assembly.FullName}\n{ex}");
        }

        return false;

        static int GetPriority(MethodInfo method)
        {
            PluginPriority? attr = method.GetCustomAttribute<PluginPriority>();
            return attr?.Priority ?? 0;
        }
    }

    private static void EnablePlugins()
    {
        foreach (MethodStruct method in PluginsList.SelectMany(x => x.EnableMethods).OrderByDescending(x => x.Priority))
            try
            {
                method.MethodInfo.Invoke(null, []);
                Log.Info(
                    $"Plugin {method.Info.Name} [{method.MethodInfo.Name}] written by {method.Info.Developer} enabled. v{method.Info.Version}");
            }
            catch (Exception ex)
            {
                Log.Error(
                    $"Plugin {method.Info.Name} [{method.MethodInfo.Name}] threw an exception while enabling\n{ex}");
            }
    }

    private static void Disable()
    {
        foreach (MethodStruct method in PluginsList.SelectMany(x => x.DisableMethods))
            try
            {
                method.MethodInfo.Invoke(null, []);
                Log.Info($"Plugin {method.Info.Name} [{method.MethodInfo.Name}] disabled");
            }
            catch (Exception ex)
            {
                Log.Error(
                    $"Plugin {method.Info.Name} [{method.MethodInfo.Name}] threw an exception while disabling\n{ex}");
            }
    }

    internal static void ReloadPlugins()
    {
        try
        {
            Log.Info("Plugins are reloading...");

            Disable();
            Internal.EventsManager.Loader.UnloadPlugins();
            PluginsList.Clear();

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