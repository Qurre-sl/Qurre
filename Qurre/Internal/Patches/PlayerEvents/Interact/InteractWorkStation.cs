using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using InventorySystem.Items.Firearms.Attachments;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Interact;

[HarmonyPatch(typeof(WorkstationController), nameof(WorkstationController.ServerInteract))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class InteractWorkStation
{
    [HarmonyPrefix]
    private static bool Call(WorkstationController __instance, ReferenceHub ply, byte colliderId)
    {
        Player? player = ply.GetPlayer();

        if (player is null)
            return false;

        InteractWorkStationEvent ev = new(player, __instance.GetWorkStation(), colliderId);

        ev.InvokeEvent();

        return ev.Allowed;
    }
}