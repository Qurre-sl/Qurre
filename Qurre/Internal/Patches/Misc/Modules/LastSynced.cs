using HarmonyLib;
using Mirror;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using Qurre.API;
using UnityEngine;

namespace Qurre.Internal.Patches.Misc.Modules
{
    [HarmonyPatch(typeof(FpcFromClientMessage), nameof(FpcFromClientMessage.ProcessMessage))]
    static class LastSynced
    {
        [HarmonyPrefix]
        static void Call(NetworkConnection sender)
        {
            if (ReferenceHub.TryGetHubNetID(sender.identity.netId, out var hub))
                hub.GetPlayer().LastSynced = Time.time;
        }
    }
}