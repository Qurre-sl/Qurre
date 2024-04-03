using HarmonyLib;
using PlayerRoles.PlayableScps.Scp079;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Scp.Scp079
{
    [HarmonyPatch(typeof(Scp079Recontainer), nameof(Scp079Recontainer.Recontain))]
    static class Recontain
    {
        [HarmonyPrefix]
        static void Call()
        {
            new Scp079RecontainEvent().InvokeEvent();
        }
    }
}