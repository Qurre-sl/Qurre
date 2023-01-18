using System;
using System.IO;

namespace Qurre.API
{
    public static class Paths
    {
        static Paths()
        {
            Assemblies = Path.Combine(Path.Combine(Environment.CurrentDirectory, "SCPSL_Data"), "Managed");
            AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Qurre = Path.Combine(AppData, "Qurre");
            Configs = Path.Combine(Qurre, "Configs");
            Logs = Path.Combine(Qurre, "Logs");
            Plugins = Path.Combine(Qurre, "Plugins");
            Dependencies = Path.Combine(Plugins, "Dependencies");
        }

        public static string Assemblies { get; }

        public static string AppData { get; }

        public static string Qurre { get; }

        public static string Configs { get; }

        public static string Logs { get; }

        public static string Plugins { get; }

        public static string Dependencies { get; }
    }
}