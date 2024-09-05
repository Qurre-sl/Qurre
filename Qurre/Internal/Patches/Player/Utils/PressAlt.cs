namespace Qurre.Internal.Patches.Player.Utils;

using HarmonyLib;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using System.Collections.Generic;
using System.Reflection.Emit;

[HarmonyPatch(typeof(FpcNoclipToggleMessage), nameof(FpcNoclipToggleMessage.ProcessMessage))]
static class PressAlt
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_1); // sender [NetworkConnection]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PressAlt), nameof(PressAlt.Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    static void Invoke(NetworkConnection sender)
    {
        Player pl = sender.identity.netId.GetPlayer();

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