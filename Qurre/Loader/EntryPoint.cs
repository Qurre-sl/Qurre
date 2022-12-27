using Qurre.API;
namespace Qurre.Loader
{
    internal class EntryPoint : ICharacterLoader
    {
        public void Init()
        {
            Log.Info("Initializing Qurre...");

            Configs.Setup();

            CustomNetworkManager.Modded = true;

            Internal.EventsManager.Loader.PathQurreEvents();

            Plugins.Init();
        }
    }
}