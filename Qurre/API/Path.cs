using System;
using CodePath = System.IO.Path;
namespace Qurre.API
{
    static public class Path
    {
        static public string AppData { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static public string Qurre { get; } = CodePath.Combine(AppData, "Qurre");

        static public string Plugins { get; } = CodePath.Combine(Qurre, "Plugins");
        static public string Dependencies { get; } = CodePath.Combine(Plugins, "Dependencies");

        static public string Configs { get; } = CodePath.Combine(Qurre, "Configs");
        static public string CustomConfigs { get; } = CodePath.Combine(Configs, "Custom");

        static public string Logs { get; } = CodePath.Combine(Qurre, "Logs");

        public static string Assemblies { get; private set; } = CodePath.Combine(CodePath.Combine(Environment.CurrentDirectory, "SCPSL_Data"), "Managed");
    }
}