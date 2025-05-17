using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Usables;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.PlayerEvents.Items;

[HarmonyPatch(typeof(UsableItemsController), nameof(UsableItemsController.ServerReceivedStatus))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class UseItem
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        LocalBuilder cancelUseEvent = generator.DeclareLocal(typeof(CancelUseItemEvent));
        LocalBuilder useEvent = generator.DeclareLocal(typeof(UseItemEvent));

        List<CodeInstruction> list = [.. instructions];
        list.Last().labels.Add(retLabel);

        int cancelIndex = list.FindLastIndex(ins =>
                              ins.opcode == OpCodes.Callvirt && ins.operand is MethodBase
                              {
                                  Name: nameof(UsableItem.OnUsingCancelled)
                              }) -
                          3;

        if (cancelIndex < 0)
        {
            Log.Error($"Creating Patch error: <Player> {{Items}} [UseItem]: Cancel Index - {cancelIndex} < 0");
            return list.AsEnumerable();
        }

        // CancelUseItemEvent
        list.InsertRange(cancelIndex,
        [
            // Player
            new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(list[cancelIndex]), // referenceHub
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            // Item
            new CodeInstruction(OpCodes.Ldloc_2), // handler [PlayerHandler]
            new CodeInstruction(OpCodes.Ldflda,
                AccessTools.Field(typeof(PlayerHandler), nameof(PlayerHandler.CurrentUsable))), // CurrentUsable
            new CodeInstruction(OpCodes.Ldfld,
                AccessTools.Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.Item))), // Item [UsableItem]

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(CancelUseItemEvent))[0]),
            new CodeInstruction(OpCodes.Stloc, cancelUseEvent.LocalIndex), // var cancelUseEvent = ...;

            new CodeInstruction(OpCodes.Ldloc_S, cancelUseEvent.LocalIndex), // cancelUseEvent.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            // if(!cancelUseEvent.Allowed) return;
            new CodeInstruction(OpCodes.Ldloc_S, cancelUseEvent.LocalIndex),
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(CancelUseItemEvent), nameof(CancelUseItemEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);


        int useIndex = list.FindLastIndex(ins =>
                           ins.opcode == OpCodes.Callvirt && ins.operand is MethodBase
                           {
                               Name: nameof(UsableItem.OnUsingStarted)
                           }) -
                       10;

        if (useIndex < 0)
        {
            Log.Error($"Creating Patch error: <Player> {{Items}} [UseItem]: Use Index - {useIndex} < 0");
            return list.AsEnumerable();
        }

        // UseItemEvent
        list.InsertRange(useIndex,
        [
            // Player
            new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(list[useIndex]), // referenceHub
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            // Item
            new CodeInstruction(OpCodes.Ldloc_1), // usableItem

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UseItemEvent))[0]),
            new CodeInstruction(OpCodes.Stloc, useEvent.LocalIndex), // var UseEvent = ...;

            new CodeInstruction(OpCodes.Ldloc_S, useEvent.LocalIndex), // UseEvent.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            // if(!UseEvent.Allowed) return;
            new CodeInstruction(OpCodes.Ldloc_S, useEvent.LocalIndex),
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(UseItemEvent), nameof(UseItemEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        return list.AsEnumerable();
    }
}