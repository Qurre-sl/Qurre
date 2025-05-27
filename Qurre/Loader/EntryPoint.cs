using System;
using System.Linq;
using JetBrains.Annotations;
using Qurre.API;
using Qurre.API.Addons;
using Qurre.Internal.EventsManager;

namespace Qurre.Loader;

[UsedImplicitly]
internal class EntryPoint : ICharacterLoader
{
    public void Disable()
    {
    }

    public void Enable()
    {
        if (StartupArgs.Args.Any(arg => string.Equals(arg, "-disableAnsiColors", StringComparison.OrdinalIgnoreCase)))
            BetterColors.Enabled = false;

        Log.Info("Initializing Qurre...");

        try
        {
            Configs.Setup();

            CustomNetworkManager.Modded = true;

            Internal.EventsManager.Loader.PathQurreEvents();

            SelfInvokeExecutor.InvokeAll();
            Prefabs.Init();
            Plugins.Init();

            Log.Custom(BetterColors.Bold($"Qurre {BetterColors.BrightRed($"v{Core.Version}")} enabled"), "Loader",
                ConsoleColor.Red);
        }
        catch (Exception e)
        {
            ServerConsole.AddLog(e.ToString(), ConsoleColor.Red);
        }
    }
}