using Qurre.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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