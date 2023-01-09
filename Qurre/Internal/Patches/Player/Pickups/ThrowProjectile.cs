using HarmonyLib;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Pickups
{
    using Qurre.API;
    using Qurre.API.Controllers;
    using Qurre.Events.Structs;

    [HarmonyPatch(typeof(ThrowableItem), nameof(ThrowableItem.ServerProcessThrowConfirmation))]
    static class ThrowProjectile
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            LocalBuilder @event = generator.DeclareLocal(typeof(ThrowProjectileEvent));

            List<CodeInstruction> list = new(instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name == nameof(ThrowableItem.ServerThrow)) - 8;

            if (index < 0)
            {
                Log.Error($"Creating Patch error: <Player> {{Pickups}} [ThrowProjectile]: Index - {index} < 0");
                return list.AsEnumerable();
            }

            Label methodLabel = generator.DefineLabel();
            var labels = list[index].ExtractLabels();
            list[index].labels.Add(methodLabel);

            list.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner))),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Item), nameof(Item.SafeGet))),

                new CodeInstruction(OpCodes.Ldloc_S, 4), // ProjectileSettings
                new CodeInstruction(OpCodes.Ldarg_1), // fullForce [bool]

                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(ThrowProjectileEvent))[0]),
                new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

                new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                // if(!@event.Allowed) return;
                new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ThrowProjectileEvent), nameof(ThrowProjectileEvent.Allowed))),
                new CodeInstruction(OpCodes.Brtrue, methodLabel),


                // fix inventory; idk why nw just ignored this in nwapi
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThrowProjectile), nameof(ThrowProjectile.CancelUse))),
                new CodeInstruction(OpCodes.Br, retLabel),
            });

            return list.AsEnumerable();
        }

        static void CancelUse(ThrowableItem @base)
        {
            @base.OwnerInventory.NetworkCurItem = ItemIdentifier.None;
            @base.OwnerInventory.UserInventory.Items.Remove(@base.ItemSerial);
            @base.OwnerInventory.ServerSendItems();
            @base.OwnerInventory.UserInventory.Items.Add(@base.ItemSerial, @base);
            @base.OwnerInventory.ServerSendItems();
        }
    }
}