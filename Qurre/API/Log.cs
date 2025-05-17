using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using Qurre.API.Addons;

namespace Qurre.API;

[PublicAPI]
public static class Log
{
    // ‑- Публичные флаги ----------------------------------------------------

    internal static bool Debugging { get; set; } = true;
    internal static bool Logging { get; set; }
    internal static bool AllLogging { get; set; }
    internal static bool Errored { get; private set; }

    // ‑- Публичное API ------------------------------------------------------

    public static void Info(object message)
    {
        Write("INFO", BetterColors.BrightYellow, ConsoleColor.Yellow, message);
    }

    public static void Debug(object message)
    {
        Write("DEBUG", BetterColors.Green, ConsoleColor.DarkGreen, message, Debugging);
    }

    public static void Warn(object message)
    {
        Write("WARN", BetterColors.Yellow, ConsoleColor.DarkYellow, message, logToFile: true);
    }

    public static void Error(object message)
    {
        Errored = true;
        Write("ERROR", BetterColors.Red, ConsoleColor.Red, message, logToFile: true, highlight: BetterColors.BrightRed);
    }

    public static void Custom(object message, string prefix = "Custom", ConsoleColor color = ConsoleColor.Gray)
    {
        Write(prefix, BetterColors.BrightBlue, color, message);
    }

    // ‑- Внутренняя реализация ---------------------------------------------

    private static void Write(
        string prefix,
        Func<string, string> colorizer,
        ConsoleColor consoleColor,
        object message,
        bool enabled = true,
        bool logToFile = false,
        Func<string, string>? highlight = null)
    {
        if (!enabled) return;

        string caller = GetCallerName();

        // Формируем строку для консоли
        string consoleLine = BetterColors.White(
            $"[{colorizer(prefix)}] [{BetterColors.BrightMagenta(caller)}] " +
            $"{(highlight is null ? message : highlight.Invoke(message.ToString()))}");

        ServerConsole.AddLog(consoleLine, consoleColor);

        // Запись в файлы
        if (logToFile) WriteToFile($"{prefix.ToUpper()}", caller, message);
        if (AllLogging) WriteToFile($"{prefix.ToUpper()}", caller, message, true);
    }

    private static string GetCallerName()
    {
        try
        {
            return Assembly.GetCallingAssembly().GetName().Name ?? "█████";
        }
        catch
        {
            return "█████";
        }
    }

    private static void WriteToFile(string level, string caller, object message, bool allLogs = false)
    {
        if (!(allLogs ? AllLogging : Logging)) return;

        EnsureLogsDirectory();

        string fileName = allLogs
            ? $"{Server.Port}-all-logs.txt"
            : $"{Server.Port}-log.txt";

        File.AppendAllText(Path.Combine(Paths.Logs, fileName),
            $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] [{level}] [{caller}] {message}{Environment.NewLine}");
    }

    private static void EnsureLogsDirectory()
    {
        if (Directory.Exists(Paths.Logs)) return;

        Directory.CreateDirectory(Paths.Logs);
        Custom($"Logs directory not found. Creating: {Paths.Logs}", BetterColors.Yellow("WARN"),
            ConsoleColor.DarkYellow);
    }
}