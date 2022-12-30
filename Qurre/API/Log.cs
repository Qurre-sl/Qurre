using System;
using System.IO;
using System.Reflection;
namespace Qurre.API
{
	static public class Log
	{
		static internal bool Debugging { get; set; } = true;
		static internal bool Logging { get; set; } = false;
		static internal bool AllLogging { get; set; } = false;

		static public void Info(object message)
		{
			string caller = "█████";
			try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }
			ServerConsole.AddLog($"[INFO] [{caller}] {message}", ConsoleColor.Yellow);
		}
		static public void Debug(object message)
		{
			if (!Debugging) return;
			string caller = "█████";
			try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }
			ServerConsole.AddLog($"[DEBUG] [{caller}] {message}", ConsoleColor.DarkGreen);
		}
		static public void Warn(object message)
		{
			string caller = "█████";
			try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }
			string text = $"[WARN] [{caller}] {message}";
			ServerConsole.AddLog(text, ConsoleColor.DarkYellow);
			LogTxt(text);
		}
		static public void Error(object message)
		{
			string caller = "█████";
			try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }
			string text = $"[ERROR] [{caller}] {message}";
			ServerConsole.AddLog(text, ConsoleColor.Red);
			LogTxt(text);
		}
		static public void Custom(object message, string prefix = "Custom", ConsoleColor color = ConsoleColor.Gray)
		{
			string caller = "█████";
			try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }
			ServerConsole.AddLog($"[{prefix}] [{caller}] {message}", color);
		}

		static internal void LogTxt(object message)
		{
			if (!Logging) return;
			if (!Directory.Exists(Path.Logs))
			{
				Directory.CreateDirectory(Path.Logs);
				Custom($"Logs directory not found - creating: {Path.Logs}", "WARN", ConsoleColor.DarkYellow);
			}
			File.AppendAllText(System.IO.Path.Combine(Path.Logs, $"{Server.Port}-log.txt"), $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}\n");
		}
		static internal void AllLogsTxt(object message)
		{
			if (!AllLogging) return;
			if (!Directory.Exists(Path.Logs))
			{
				Directory.CreateDirectory(Path.Logs);
				Custom($"Logs directory not found - creating: {Path.Logs}", "WARN", ConsoleColor.DarkYellow);
			}
			File.AppendAllText(System.IO.Path.Combine(Path.Logs, $"{Server.Port}-all-logs.txt"), $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}\n");
		}
	}
}