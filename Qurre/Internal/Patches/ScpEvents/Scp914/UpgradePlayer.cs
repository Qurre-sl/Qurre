using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using Scp914;
using Scp914.Processors;
using UnityEngine;

namespace Qurre.Internal.Patches.ScpEvents.Scp914;

[HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPlayer))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class UpgradePlayer
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // ply [ReferenceHub]
        yield return new CodeInstruction(OpCodes.Ldarg_1); // upgradeInventory [bool]
        yield return new CodeInstruction(OpCodes.Ldarg_2); // heldOnly [bool]
        yield return new CodeInstruction(OpCodes.Ldarg_3); // moveVector [Vector3]
        yield return new CodeInstruction(OpCodes.Ldarg_S, 4); // setting [Scp914KnobSetting]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UpgradePlayer), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static void Invoke(ReferenceHub ply, bool upgradeInventory, bool heldOnly, Vector3 moveVector,
        Scp914KnobSetting setting)
    {
        if (Physics.Linecast(ply.transform.position, Scp914Controller.Singleton.IntakeChamber.position,
                Scp914Upgrader.SolidObjectMask))
            return;

        Player? pl = ply.GetPlayer();

        if (pl is null)
            return;

        Scp914UpgradePlayerEvent ev = new(pl, null, null, upgradeInventory, heldOnly, moveVector, setting);

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var pareItem in ply.inventory.UserInventory.Items)
        {
            if (!pareItem.Value)
                continue;

            ev.Inventory.Add(pareItem.Value);
        }

        ev.InvokeEvent();

        if (!ev.Allowed)
            return;

        ply.TryOverridePosition(ev.TargetPosition, Vector3.zero);

        if (!ev.UpgradeInventory)
            return;

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (ItemBase? item in ev.Inventory)
            if (!ev.HeldOnly || item.ItemSerial == ply.inventory.CurItem.SerialNumber)
                ev.InstantUpgrade.Add(item);

        foreach (ItemBase? upItem in ev.InstantUpgrade)
        {
            if (!Scp914Upgrader.TryGetProcessor(upItem.ItemTypeId, out Scp914ItemProcessor? processor))
                continue;

            Scp914Upgrader.OnInventoryItemUpgraded?.Invoke(upItem, ev.Setting);
            processor.OnInventoryItemUpgraded(ev.Setting, ply, upItem.ItemSerial);
        }

        HashSetPool<ItemBase>.Shared.Return(ev.Inventory);
        HashSetPool<ItemBase>.Shared.Return(ev.InstantUpgrade);

        BodyArmorUtils.RemoveEverythingExceedingLimits(ply.inventory,
            ply.inventory.TryGetBodyArmor(out BodyArmor? bodyArmor) ? bodyArmor : null);
    }
}