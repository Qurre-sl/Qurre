using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
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
            ServerConsole.AddLog($"[LoadDependencies] {ex}", ConsoleColor.Red);
        }

        try
        {
            PatchMethods();
        }
        catch (Exception ex)
        {
            ServerConsole.AddLog($"[PatchMethods] {ex}", ConsoleColor.Red);
        }

        try
        {
            LoadPlugins();
        }
        catch (Exception ex)
        {
            ServerConsole.AddLog($"[LoadPlugins] {ex}", ConsoleColor.Red);
        }

        try
        {
            Internal.EventsManager.Loader.SortMethods();
        }
        catch (Exception ex)
        {
            ServerConsole.AddLog($"[SortMethods] {ex}", ConsoleColor.Red);
        }

        try
        {
            EnablePlugins();
        }
        catch (Exception ex)
        {
            ServerConsole.AddLog($"[EnablePlugins] {ex}", ConsoleColor.Red);
        }
    }

    private static void PatchMethods()
    {
        try
        {
            bool errored = false;
            _harmony = new Harmony("qurre.patches");

            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
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
            }

            if (!errored)
                Log.Info("Harmony successfully Patched");
        }
        catch (Exception ex)
        {
            Log.Error($"Harmony Patching threw an error:\n{ex}");
        }
    }

    private static void LoadDependencies()
    {
        if (!Directory.Exists(Paths.Depends))
            Directory.CreateDirectory(Paths.Depends);

        List<string> files = [.. Directory.GetFiles(Paths.Depends)];

        if (Paths.CustomDepends != Paths.Depends &&
            Directory.Exists(Paths.CustomDepends))
            files.AddRange(Directory.GetFiles(Paths.CustomDepends));

        if (Directory.Exists(Paths.SystemDepends))
            files.AddRange(Directory.GetFiles(Paths.SystemDepends));

        foreach (string dll in files)
        {
            if (!dll.EndsWith(".dll") || Manager.Loaded(dll)) continue;

            Assembly assembly = Manager.LoadAssembly(dll);

            Log.Custom("Loaded dependency " + assembly.FullName, "Loader", ConsoleColor.Blue);
        }

        Log.Custom("Depends loaded", "Loader", ConsoleColor.Green);
    }

    private static void LoadPlugins()
    {
        List<string> files = [.. Directory.GetFiles(Paths.Plugins)];

        if (Paths.CustomPlugins != Paths.Plugins &&
            Directory.Exists(Paths.CustomPlugins))
            files.AddRange(Directory.GetFiles(Paths.CustomPlugins));

        foreach (string plugin in files)
            try
            {
                Log.Debug($"Loading {plugin}");
                Assembly assembly = Manager.LoadAssembly(plugin);
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
        foreach (var method in PluginsList.SelectMany(ps => ps.EnableMethods).OrderByDescending(ms => ms.Priority))
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
        foreach (var method in PluginsList.SelectMany(ps => ps.DisableMethods))
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