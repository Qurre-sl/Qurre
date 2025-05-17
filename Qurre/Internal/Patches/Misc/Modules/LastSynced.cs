using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Mirror;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using Qurre.API;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.Internal.Patches.Misc.Modules;

[HarmonyPatch(typeof(FpcFromClientMessage), nameof(FpcFromClientMessage.ProcessMessage))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class LastSynced
{
    [HarmonyPrefix]
    private static void Call(NetworkConnection sender)
    {
        if (!ReferenceHub.TryGetHubNetID(sender.identity.netId, out ReferenceHub? hub))
            return;

        Player? player = hub.GetPlayer();

        if (player is null)
            return;

        player.LastSynced = Time.time;
    }
}