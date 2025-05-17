using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using Qurre.API;
using Qurre.API.Utils.Entities;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Pickups;

[HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.ValidateStart))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class PrePickupItem
{
    
    [HarmonyPostfix, SuppressMessage("ReSharper", "InconsistentNaming")]
    private static void Postfix(ItemSearchCompletor __instance, ref bool __result)
    {
        var player = __instance.Hub.GetPlayer();
        var pickup = ItemsHelper.GetPickupByBase(__instance.TargetPickup);
        var ev = new PrePickupItemEvent(player, pickup);
        ev.InvokeEvent();

        if (!ev.IsAllowed)
            __result = false;
    }
    
    // TODO: check postfix pre-pickup variant
    
    /*
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var returnLabel = generator.DefineLabel();

        var ev = generator.DeclareLocal(typeof(PrePickupItemEvent));

        var list = new List<CodeInstruction>(instructions);
        list.Last().labels.Add(returnLabel);

        var label = generator.DefineLabel();
        var labels = list[^2].ExtractLabels();
        list[^2].labels.Add(label); // list.Count - 2

        list.InsertRange(list.Count - 2,
        [
            new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PickupSearchCompletor), nameof(PickupSearchCompletor.Hub))),
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld,
                AccessTools.Field(typeof(PickupSearchCompletor), nameof(PickupSearchCompletor.TargetPickup))),
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(ItemsHelper), nameof(ItemsHelper.GetPickupByBase), [typeof(ItemPickupBase)])),

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(PrePickupItemEvent))[0]),
            new CodeInstruction(OpCodes.Stloc, ev.LocalIndex), // var ev = ...;

            new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex), // ev.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            // if !ev.Allowed return;
            new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(PrePickupItemEvent), nameof(PrePickupItemEvent.IsAllowed))),
            new CodeInstruction(OpCodes.Brtrue, label),
            new CodeInstruction(OpCodes.Ldc_I4_0), // false [if boolean]
            new CodeInstruction(OpCodes.Br, returnLabel)
        ]);

        return list.AsEnumerable();
    }
    */
}
/*
 * ...
 * PrePickupItemEvent @event = new(this.Hub.GetPlayer(), Pickup.SafeGet(this.TargetPickup));
 * @event.InvokeEvent();
 *
 * if(!@event.Allowed)
 *     return false;
 *
 * return true;
 */