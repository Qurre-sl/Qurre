using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Disarming;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.PlayerEvents.Socialization;

[HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Cuff
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        LocalBuilder unCuffEvent = generator.DeclareLocal(typeof(UnCuffEvent));
        LocalBuilder cuffEvent = generator.DeclareLocal(typeof(CuffEvent));

        List<CodeInstruction> list = [.. instructions];
        list.Last().labels.Add(retLabel);

        int unCuffIndex = list.FindIndex(ins => ins.opcode == OpCodes.Call &&
                                                ins.operand is MethodBase
                                                {
                                                    Name: nameof(DisarmedPlayers.SetDisarmedStatus)
                                                }) - 4;

        if (unCuffIndex < 0)
        {
            Log.Error($"Creating Patch error: <Player> {{Socialization}} [Cuff]: UnCuff Index - {unCuffIndex} < 0");
            return list.AsEnumerable();
        }

        list.InsertRange(unCuffIndex,
        [
            new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(list[unCuffIndex]), // msg [DisarmMessage]
            new CodeInstruction(OpCodes.Ldfld,
                AccessTools.Field(typeof(DisarmMessage),
                    nameof(DisarmMessage.PlayerToDisarm))), // target [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Ldloc_0), // cuffer [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UnCuffEvent))[0]),
            new CodeInstruction(OpCodes.Stloc_S, unCuffEvent.LocalIndex), // var unCuffEvent = ...;

            new CodeInstruction(OpCodes.Ldloc_S, unCuffEvent.LocalIndex), // unCuffEvent.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            new CodeInstruction(OpCodes.Ldloc_S, unCuffEvent.LocalIndex), // if(!unCuffEvent.Allowed) return;
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(UnCuffEvent), nameof(UnCuffEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);


        int cuffIndex = list.FindLastIndex(ins =>
            ins.opcode == OpCodes.Call && ins.operand is MethodBase
            {
                Name: nameof(DisarmedPlayers.SetDisarmedStatus)
            }) - 5;

        if (cuffIndex < 0)
        {
            Log.Error($"Creating Patch error: <Player> {{Socialization}} [Cuff]: Cuff Index - {cuffIndex} < 0");
            return list.AsEnumerable();
        }

        list.InsertRange(cuffIndex,
        [
            new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(list[cuffIndex]), // msg [DisarmMessage]
            new CodeInstruction(OpCodes.Ldfld,
                AccessTools.Field(typeof(DisarmMessage),
                    nameof(DisarmMessage.PlayerToDisarm))), // target [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Ldloc_0), // cuffer [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(CuffEvent))[0]),
            new CodeInstruction(OpCodes.Stloc_S, cuffEvent.LocalIndex), // var cuffEvent = ...;

            new CodeInstruction(OpCodes.Ldloc_S, cuffEvent.LocalIndex), // cuffEvent.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            new CodeInstruction(OpCodes.Ldloc_S, cuffEvent.LocalIndex), // if(!cuffEvent.Allowed) return;
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(CuffEvent), nameof(CuffEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel),

            new CodeInstruction(OpCodes.Ldloc_S,
                cuffEvent.LocalIndex), // {cuffer} referenceHub = cuffEvent.Cuffer.Role;
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(CuffEvent), nameof(CuffEvent.Cuffer))),
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(Player), nameof(Player.ReferenceHub))),
            new CodeInstruction(OpCodes.Stloc_0)
        ]);

        return list.AsEnumerable();
    }
}