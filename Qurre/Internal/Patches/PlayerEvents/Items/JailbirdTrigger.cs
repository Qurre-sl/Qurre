using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Jailbird;
using Mirror;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Items;

[HarmonyPatch(typeof(JailbirdItem), nameof(JailbirdItem.ServerProcessCmd))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class JailbirdTrigger
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> list = [.. instructions];

        int index = list.FindLastIndex(ins => ins.opcode == OpCodes.Stloc_0) + 1;

        if (index < 1)
        {
            Log.Error($"Creating Patch error: <Player> {{Items}} [JailbirdTrigger]: Index - {index} < 1");
            return list.AsEnumerable();
        }

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldarg_0), // @base [JailbirdItem]
            new CodeInstruction(OpCodes.Ldloc_0), // message [JailbirdMessageType]
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(JailbirdTrigger), nameof(Invoke))),
            new CodeInstruction(OpCodes.Stloc_0)
        ]);

        return list.AsEnumerable();
    }

    private static JailbirdMessageType Invoke(JailbirdItem @base, JailbirdMessageType message)
    {
        if (message is JailbirdMessageType.ChargeStarted && @base._charging &&
            NetworkTime.time - @base._chargeResetTime < .5)
            return JailbirdMessageType.UpdateState;

        Player? player = @base.Owner.GetPlayer();

        if (player is null)
            return message;

        JailbirdTriggerEvent @event = new(player, @base, message);
        @event.InvokeEvent();

        if (@event.Allowed)
            return @event.Message;

        @event.Message = JailbirdMessageType.UpdateState;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (message)
        {
            case JailbirdMessageType.ChargeStarted:
                @base._charging = true;
                @base._firstChargeFrame = true;
                @base._chargeLoadStopwatch.Reset();
                @base._chargeAnyDetected = false;
                @base._chargeResetTime = NetworkTime.time;
                @base.SendRpc(JailbirdMessageType.ChargeStarted,
                    delegate (NetworkWriter wr) { wr.WriteDouble(@base._chargeResetTime); });
                @base.SendRpc(JailbirdMessageType.ChargeFailed);
                break;
            case JailbirdMessageType.ChargeFailed:
                @event.Message = JailbirdMessageType.ChargeFailed;
                break;
        }

        return @event.Message;
    }
}