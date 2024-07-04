using HarmonyLib;
using InventorySystem.Items.Jailbird;
using Mirror;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Items
{
    [HarmonyPatch(typeof(JailbirdItem), nameof(JailbirdItem.ServerProcessCmd))]
    static class JailbirdTrigger
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new(instructions);

            int index = list.FindLastIndex(ins => ins.opcode == OpCodes.Stloc_0) + 1;

            if (index < 1)
            {
                Log.Error($"Creating Patch error: <Player> {{Items}} [JailbirdTrigger]: Index - {index} < 1");
                return list.AsEnumerable();
            }

            list.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0), // @base [JailbirdItem]
                new(OpCodes.Ldloc_0), // message [JailbirdMessageType]
                new(OpCodes.Call, AccessTools.Method(typeof(JailbirdTrigger), nameof(JailbirdTrigger.Invoke))),
                new(OpCodes.Stloc_0),
            });

            return list.AsEnumerable();
        }

        static JailbirdMessageType Invoke(JailbirdItem @base, JailbirdMessageType message)
        {
            if (message is JailbirdMessageType.ChargeStarted && @base._charging && (NetworkTime.time - @base._chargeResetTime) < .5)
                return JailbirdMessageType.UpdateState;

            JailbirdTriggerEvent @event = new(@base.Owner.GetPlayer(), @base, message);
            @event.InvokeEvent();

            if (!@event.Allowed)
            {
                @event.Message = JailbirdMessageType.UpdateState;

                if (message is JailbirdMessageType.ChargeStarted)
                {
                    @base._charging = true;
                    @base._firstChargeFrame = true;
                    @base._chargeLoading = false;
                    @base._chargeAnyDetected = false;
                    @base._chargeResetTime = NetworkTime.time;
                    @base.SendRpc(JailbirdMessageType.ChargeStarted, delegate (NetworkWriter wr)
                    {
                        wr.WriteDouble(@base._chargeResetTime);
                    });
                    @base.SendRpc(JailbirdMessageType.ChargeFailed);
                }
                else if (message is JailbirdMessageType.ChargeFailed)
                    @event.Message = JailbirdMessageType.ChargeFailed;
            }

            return @event.Message;
        }
    }
}