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

    [HarmonyPatch(typeof(UsableItemsController), nameof(UsableItemsController.Update))]
    static class UsedItem
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new(instructions);

            int index = list.FindLastIndex(ins => ins.opcode == OpCodes.Callvirt && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name == nameof(UsableItem.ServerOnUsingCompleted)) - 2;

            if (index < 0)
            {
                Log.Error($"Creating Patch error: <Player> {{Items}} [UsedItem]: Index - {index} < 0");
                return list.AsEnumerable();
            }

            list.InsertRange(index, new CodeInstruction[]
            {
                // Player
                new CodeInstruction(OpCodes.Ldloca_S, 1).MoveLabelsFrom(list[index]), // KeyValuePair<ReferenceHub, ...> keyValuePair
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(
                    typeof(KeyValuePair<ReferenceHub, PlayerHandler>),
                    nameof(KeyValuePair<ReferenceHub, PlayerHandler>.Key)
                )), // ReferenceHub
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),

                // Item
                new CodeInstruction(OpCodes.Ldloc_2), // currentUsable [CurrentlyUsedItem]
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.Item))), // Item [UsableItem]
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Item), nameof(Item.SafeGet))),

                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UsedItemEvent))[0]),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),
            });


            // remove "return" in nwapi event, because "if(!allowed) break;" doesn't work, bruh
            int DelIndex = list.FindLastIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name.Contains("ExecuteEvent"));

            if (DelIndex < 0)
            {
                Log.Error($"Creating Patch error: <Player> {{Items}} [UsedItem]: Delete Index - {DelIndex} < 0");
                return list.AsEnumerable();
            }

            list[DelIndex + 1].opcode = OpCodes.Br_S;

            return list.AsEnumerable();
        }
    }
}