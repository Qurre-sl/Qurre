using System;
using System.IO;
using JetBrains.Annotations;

namespace Qurre.API;

[PublicAPI]
public static class Paths
{
    public static string AppData { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static string Qurre { get; } = Path.Combine(AppData, "Qurre");

    public static string Plugins { get; } = Path.Combine(Qurre, "Plugins");
    public static string Depends { get; } = Path.Combine(Plugins, "Depends");
    public static string SystemDepends { get; } = Path.Combine(Depends, "System");

    public static string CustomPlugins { get; private set; } = Path.Combine(Qurre, "Plugins");
    public static string CustomDepends { get; private set; } = Path.Combine(CustomPlugins, "Depends");

    public static string Configs { get; } = Path.Combine(Qurre, "Configs");

    public static string Logs { get; } = Path.Combine(Qurre, "Logs");

    public static string Assemblies { get; } =
        Path.Combine(Path.Combine(Environment.CurrentDirectory, "SCPSL_Data"), "Managed");


    internal static void UpdatePluginsDirectory(string pluginsDirectory)
    {
        CustomPlugins = Path.Combine(Qurre, pluginsDirectory);
        CustomDepends = Path.Combine(CustomPlugins, "Depends");
    }
}