using Qurre.API;
namespace Qurre.Loader
{
    internal class EntryPoint : ICharacterLoader
    {
        static internal System.Version Version { get; private set; } = new(2, 0);

        public void Init()
        {
            Log.Info("Initializing Qurre...");

            Configs.Setup();

            CustomNetworkManager.Modded = true;

            Internal.EventsManager.Loader.PathQurreEvents();

            Plugins.Init();

            Log.Custom($"Qurre v{Version} enabled", "Loader", System.ConsoleColor.Green);
        }
    }
}