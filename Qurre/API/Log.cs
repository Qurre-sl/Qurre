namespace Qurre.API;

using Qurre.API.Addons;
using System;
using System.IO;
using System.Reflection;

static public class Log
{
	static internal bool Debugging { get; set; } = true;
	static internal bool Logging { get; set; } = false;
	static internal bool AllLogging { get; set; } = false;
	static internal bool Errored { get; private set; } = false;


	static public void Info(object message)
	{
		string caller = "█████";
		try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }

		ServerConsole.AddLog(BetterColors.White($"[{BetterColors.BrightYellow("INFO")}] " +
			$"[{BetterColors.BrightMagenta(caller)}] {message}"), ConsoleColor.Yellow);
	}

	static public void Debug(object message)
	{
		if (!Debugging)
			return;

		string caller = "█████";
		try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }

		ServerConsole.AddLog(BetterColors.White($"[{BetterColors.Green("DEBUG")}] " +
			$"[{BetterColors.BrightMagenta(caller)}] {message}"), ConsoleColor.DarkGreen);
	}

	static public void Warn(object message)
	{
		string caller = "█████";
		try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }

		ServerConsole.AddLog(BetterColors.White($"[{BetterColors.Yellow("WARN")}] " +
			$"[{BetterColors.BrightMagenta(caller)}] {message}"), ConsoleColor.DarkYellow);

		LogTxt($"[WARN] [{caller}] {message}");
	}

	static public void Error(object message)
	{
		Errored = true;

		string caller = "█████";
		try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }

		ServerConsole.AddLog(BetterColors.White($"[{BetterColors.Red("ERROR")}] " +
			$"[{BetterColors.BrightMagenta(caller)}] {BetterColors.BrightRed(message)}"), ConsoleColor.Red);

		LogTxt($"[ERROR] [{caller}] {message}");
	}

	static public void Custom(object message, string prefix = "Custom", ConsoleColor color = ConsoleColor.Gray)
	{
		string caller = "█████";
		try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }

		ServerConsole.AddLog(BetterColors.White($"[{BetterColors.BrightBlue(prefix)}] " +
			$"[{BetterColors.BrightMagenta(caller)}] {message}"), color);
	}


	static internal void LogTxt(object message)
	{
		if (!Logging)
			return;

		if (!Directory.Exists(Pathes.Logs))
		{
			Directory.CreateDirectory(Pathes.Logs);
			Custom($"Logs directory not found. Creating: {Pathes.Logs}", BetterColors.Yellow("WARN"), ConsoleColor.DarkYellow);
		}

		File.AppendAllText(Path.Combine(Pathes.Logs, $"{Server.Port}-log.txt"), $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}\n");
	}

	static internal void AllLogsTxt(object message)
	{
		if (!AllLogging)
			return;

		if (!Directory.Exists(Pathes.Logs))
		{
			Directory.CreateDirectory(Pathes.Logs);
			Custom($"Logs directory not found. Creating: {Pathes.Logs}", BetterColors.Yellow("WARN"), ConsoleColor.DarkYellow);
		}

		File.AppendAllText(Path.Combine(Pathes.Logs, $"{Server.Port}-all-logs.txt"), $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}\n");
	}
}