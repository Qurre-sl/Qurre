using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using Qurre.API.Addons;

namespace Qurre.API;

[PublicAPI]
public static class Log
{
    internal static bool Debugging { get; set; } = true;
    internal static bool Logging { get; set; }
    internal static bool AllLogging { get; set; }
    internal static bool Errored { get; private set; }


    public static void Info(object message)
    {
        string caller;
        try
        {
            caller = Assembly.GetCallingAssembly().GetName().Name;
        }
        catch
        {
            caller = "█████";
        }

        ServerConsole.AddLog(BetterColors.White($"[{BetterColors.BrightYellow("INFO")}] " +
                                                $"[{BetterColors.BrightMagenta(caller)}] {message}"),
            ConsoleColor.Yellow);
    }

    public static void Debug(object message)
    {
        if (!Debugging)
            return;

        string caller;
        try
        {
            caller = Assembly.GetCallingAssembly().GetName().Name;
        }
        catch
        {
            caller = "█████";
        }

        ServerConsole.AddLog(BetterColors.White($"[{BetterColors.Green("DEBUG")}] " +
                                                $"[{BetterColors.BrightMagenta(caller)}] {message}"),
            ConsoleColor.DarkGreen);
    }

    public static void Warn(object message)
    {
        string caller;
        try
        {
            caller = Assembly.GetCallingAssembly().GetName().Name;
        }
        catch
        {
            caller = "█████";
        }

        ServerConsole.AddLog(BetterColors.White($"[{BetterColors.Yellow("WARN")}] " +
                                                $"[{BetterColors.BrightMagenta(caller)}] {message}"),
            ConsoleColor.DarkYellow);

        LogTxt($"[WARN] [{caller}] {message}");
    }

    public static void Error(object message)
    {
        Errored = true;

        string caller;
        try
        {
            caller = Assembly.GetCallingAssembly().GetName().Name;
        }
        catch
        {
            caller = "█████";
        }

        ServerConsole.AddLog(BetterColors.White($"[{BetterColors.Red("ERROR")}] " +
                                                $"[{BetterColors.BrightMagenta(caller)}] {BetterColors.BrightRed(message)}"),
            ConsoleColor.Red);

        LogTxt($"[ERROR] [{caller}] {message}");
    }

    public static void Custom(object message, string prefix = "Custom", ConsoleColor color = ConsoleColor.Gray)
    {
        string caller;
        try
        {
            caller = Assembly.GetCallingAssembly().GetName().Name;
        }
        catch
        {
            caller = "█████";
        }

        ServerConsole.AddLog(BetterColors.White($"[{BetterColors.BrightBlue(prefix)}] " +
                                                $"[{BetterColors.BrightMagenta(caller)}] {message}"), color);
    }


    internal static void LogTxt(object message)
    {
        if (!Logging)
            return;

        if (!Directory.Exists(Paths.Logs))
        {
            Directory.CreateDirectory(Paths.Logs);
            Custom($"Logs directory not found. Creating: {Paths.Logs}", BetterColors.Yellow("WARN"),
                ConsoleColor.DarkYellow);
        }

        File.AppendAllText(Path.Combine(Paths.Logs, $"{Server.Port}-log.txt"),
            $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}\n");
    }

    internal static void AllLogsTxt(object message)
    {
        if (!AllLogging)
            return;

        if (!Directory.Exists(Paths.Logs))
        {
            Directory.CreateDirectory(Paths.Logs);
            Custom($"Logs directory not found. Creating: {Paths.Logs}", BetterColors.Yellow("WARN"),
                ConsoleColor.DarkYellow);
        }

        File.AppendAllText(Path.Combine(Paths.Logs, $"{Server.Port}-all-logs.txt"),
            $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}\n");
    }
}