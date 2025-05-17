using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Utils;

[HarmonyPatch(typeof(FpcNoclipToggleMessage), nameof(FpcNoclipToggleMessage.ProcessMessage))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class PressAlt
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_1); // sender [NetworkConnection]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PressAlt), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static void Invoke(NetworkConnection sender)
    {
        Player? pl = sender.identity.netId.GetPlayer();

        if (pl is null)
            return;

        PressAltEvent @event = new(pl, FpcNoclip.IsPermitted(pl.ReferenceHub));
        @event.InvokeEvent();

        if (!@event.Allowed)
            return;

        if (pl.RoleInformation.Base is not IFpcRole)
        {
            pl.Client.SendConsole("Noclip is not supported for this class.", "yellow");
            return;
        }

        pl.HealthInformation.PlayerStats.GetModule<AdminFlagsStat>().InvertFlag(AdminFlags.Noclip);
    }
}