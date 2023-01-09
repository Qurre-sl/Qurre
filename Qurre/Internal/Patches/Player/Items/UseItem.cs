using HarmonyLib;
using InventorySystem.Items.Usables;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Items
{
    using Qurre.API;
    using Qurre.API.Controllers;
    using Qurre.Events.Structs;

    [HarmonyPatch(typeof(UsableItemsController), nameof(UsableItemsController.ServerReceivedStatus))]
    static class UseItem
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            LocalBuilder cancelUseEvent = generator.DeclareLocal(typeof(CancelUseItemEvent));
            LocalBuilder UseEvent = generator.DeclareLocal(typeof(UseItemEvent));

            List<CodeInstruction> list = new(instructions);

            int cancelIndex = list.FindLastIndex(ins => ins.opcode == OpCodes.Callvirt && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name == nameof(UsableItem.OnUsingCancelled)) - 3;

            if (cancelIndex < 0)
            {
                Log.Error($"Creating Patch error: <Player> {{Items}} [UseItem]: Cancel Index - {cancelIndex} < 0");
                return list.AsEnumerable();
            }

            // CancelUseItemEvent
            list.InsertRange(cancelIndex, new CodeInstruction[]
            {
                // Player
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(list[cancelIndex]), // referenceHub
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),

                // Item
                new CodeInstruction(OpCodes.Ldloc_2), // handler [PlayerHandler]
                new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(PlayerHandler), nameof(PlayerHandler.CurrentUsable))), // CurrentUsable
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.Item))), // Item [UsableItem]
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Item), nameof(Item.SafeGet))),

                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(CancelUseItemEvent))[0]),
                new CodeInstruction(OpCodes.Stloc, cancelUseEvent.LocalIndex), // var cancelUseEvent = ...;

                new CodeInstruction(OpCodes.Ldloc_S, cancelUseEvent.LocalIndex), // cancelUseEvent.InvokeEvent();
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                // if(!cancelUseEvent.Allowed) return;
                new CodeInstruction(OpCodes.Ldloc_S, cancelUseEvent.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(CancelUseItemEvent), nameof(CancelUseItemEvent.Allowed))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),
            });


            int useIndex = list.FindLastIndex(ins => ins.opcode == OpCodes.Callvirt && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name == nameof(UsableItem.OnUsingStarted)) - 10;

            if (useIndex < 0)
            {
                Log.Error($"Creating Patch error: <Player> {{Items}} [UseItem]: Use Index - {useIndex} < 0");
                return list.AsEnumerable();
            }

            // UseItemEvent
            list.InsertRange(useIndex, new CodeInstruction[]
            {
                // Player
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(list[useIndex]), // referenceHub
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),

                // Item
                new CodeInstruction(OpCodes.Ldloc_1), // usableItem
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Item), nameof(Item.SafeGet))),

                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UseItemEvent))[0]),
                new CodeInstruction(OpCodes.Stloc, UseEvent.LocalIndex), // var UseEvent = ...;

                new CodeInstruction(OpCodes.Ldloc_S, UseEvent.LocalIndex), // UseEvent.InvokeEvent();
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                // if(!UseEvent.Allowed) return;
                new CodeInstruction(OpCodes.Ldloc_S, UseEvent.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UseItemEvent), nameof(UseItemEvent.Allowed))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),
            });

            return list.AsEnumerable();
        }
    }
}