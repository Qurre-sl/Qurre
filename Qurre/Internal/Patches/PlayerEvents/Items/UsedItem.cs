using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Usables;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.PlayerEvents.Items;

[HarmonyPatch(typeof(UsableItemsController), nameof(UsableItemsController.Update))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class UsedItem
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> list = [..instructions];

        int index = list.FindLastIndex(ins =>
            ins.opcode == OpCodes.Callvirt && ins.operand is MethodBase
            {
                Name: nameof(UsableItem.ServerOnUsingCompleted)
            }) - 2;

        if (index < 0)
        {
            Log.Error($"Creating Patch error: <Player> {{Items}} [UsedItem]: Index - {index} < 0");
            return list.AsEnumerable();
        }

        list.InsertRange(index,
        [
            // Player
            new CodeInstruction(OpCodes.Ldloca_S, 1)
                .MoveLabelsFrom(list[index]), // KeyValuePair<ReferenceHub, ...> keyValuePair
            new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(
                typeof(KeyValuePair<ReferenceHub, PlayerHandler>),
                nameof(KeyValuePair<ReferenceHub, PlayerHandler>.Key)
            )), // ReferenceHub
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            // Item
            new CodeInstruction(OpCodes.Ldloc_2), // currentUsable [CurrentlyUsedItem]
            new CodeInstruction(OpCodes.Ldfld,
                AccessTools.Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.Item))), // Item [UsableItem]
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Item), nameof(Item.SafeGet))),

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UsedItemEvent))[0]),
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent)))
        ]);


        // remove "return" in nwapi event, because "if(!allowed) break;" doesn't work, bruh
        int delIndex = list.FindLastIndex(ins =>
            ins.opcode == OpCodes.Call && ins.operand is MethodBase methodBase &&
            methodBase.Name.Contains("ExecuteEvent"));

        if (delIndex < 0)
        {
            Log.Error($"Creating Patch error: <Player> {{Items}} [UsedItem]: Delete Index - {delIndex} < 0");
            return list.AsEnumerable();
        }

        list[delIndex + 1].opcode = OpCodes.Br_S;

        return list.AsEnumerable();
    }
}