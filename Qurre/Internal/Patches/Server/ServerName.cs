using HarmonyLib;
using Qurre.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlayerList;

namespace Qurre.Internal.Patches.Server
{
    [HarmonyPatch(typeof(ServerConsole), "ReloadServerName")]
    internal static class ServerNamePatch
    {
        internal static void Postfix()
        {
            if (!Configs.NameTracked) return;

           
        }
    }
}
