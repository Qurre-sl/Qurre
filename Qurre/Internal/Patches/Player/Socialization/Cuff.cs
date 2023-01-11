using HarmonyLib;
using InventorySystem.Disarming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Socialization
{
    using Qurre.API;
    using Qurre.Events.Structs;

    [HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
    static class Cuff
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            LocalBuilder uncuffEvent = generator.DeclareLocal(typeof(UnCuffEvent));
            LocalBuilder cuffEvent = generator.DeclareLocal(typeof(CuffEvent));

            List<CodeInstruction> list = new(instructions);

            int uncuffIndex = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name == nameof(DisarmedPlayers.SetDisarmedStatus)) - 4;

            if (uncuffIndex < 0)
            {
                Log.Error($"Creating Patch error: <Player> {{Socialization}} [Cuff]: UnCuff Index - {uncuffIndex} < 0");
                return list.AsEnumerable();
            }

            list.InsertRange(uncuffIndex, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(list[uncuffIndex]), // msg [DisarmMessage]
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))), // target [ReferenceHub]
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new Type[] { typeof(ReferenceHub) })),

                new CodeInstruction(OpCodes.Ldloc_0), // cuffer [ReferenceHub]
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new Type[] { typeof(ReferenceHub) })),

                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UnCuffEvent))[0]),
                new CodeInstruction(OpCodes.Stloc_S, uncuffEvent.LocalIndex), // var uncuffEvent = ...;

                new CodeInstruction(OpCodes.Ldloc_S, uncuffEvent.LocalIndex), // uncuffEvent.InvokeEvent();
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                new CodeInstruction(OpCodes.Ldloc_S, uncuffEvent.LocalIndex), // if(!uncuffEvent.Allowed) return;
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnCuffEvent), nameof(UnCuffEvent.Allowed))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),
            });


            int cuffIndex = list.FindLastIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name == nameof(DisarmedPlayers.SetDisarmedStatus)) - 5;

            if (cuffIndex < 0)
            {
                Log.Error($"Creating Patch error: <Player> {{Socialization}} [Cuff]: Cuff Index - {cuffIndex} < 0");
                return list.AsEnumerable();
            }

            list.InsertRange(cuffIndex, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(list[cuffIndex]), // msg [DisarmMessage]
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))), // target [ReferenceHub]
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new Type[] { typeof(ReferenceHub) })),

                new CodeInstruction(OpCodes.Ldloc_0), // cuffer [ReferenceHub]
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new Type[] { typeof(ReferenceHub) })),

                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(CuffEvent))[0]),
                new CodeInstruction(OpCodes.Stloc_S, cuffEvent.LocalIndex), // var cuffEvent = ...;

                new CodeInstruction(OpCodes.Ldloc_S, cuffEvent.LocalIndex), // cuffEvent.InvokeEvent();
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                new CodeInstruction(OpCodes.Ldloc_S, cuffEvent.LocalIndex), // if(!cuffEvent.Allowed) return;
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(CuffEvent), nameof(CuffEvent.Allowed))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),

                new CodeInstruction(OpCodes.Ldloc_S, cuffEvent.LocalIndex), // {cuffer} referenceHub = cuffEvent.Cuffer.Role;
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(CuffEvent), nameof(CuffEvent.Cuffer))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Player), nameof(Player.ReferenceHub))),
                new CodeInstruction(OpCodes.Stloc_0),
            });

            list.InsertRange(cuffIndex, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Nop).MoveLabelsFrom(list[cuffIndex]),
            });

            return list.AsEnumerable();
        }
    }
}