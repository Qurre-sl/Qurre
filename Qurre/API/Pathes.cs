namespace Qurre.API;

using System;
using System.IO;

static public class Pathes
{
    static public string AppData { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    static public string Qurre { get; } = Path.Combine(AppData, "Qurre");

    static public string Plugins { get; } = Path.Combine(Qurre, "Plugins");
    static public string Dependencies { get; } = Path.Combine(Plugins, "Dependencies");

    static public string Configs { get; } = Path.Combine(Qurre, "Configs");

    static public string Logs { get; } = Path.Combine(Qurre, "Logs");

    public static string Assemblies { get; private set; } = Path.Combine(Path.Combine(Environment.CurrentDirectory, "SCPSL_Data"), "Managed");
}