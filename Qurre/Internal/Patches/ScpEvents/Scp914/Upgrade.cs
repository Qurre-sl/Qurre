using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Pickups;
using Mirror;
using NorthwoodLib.Pools;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using Scp914;
using UnityEngine;

namespace Qurre.Internal.Patches.ScpEvents.Scp914;

[HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.Upgrade))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Upgrade
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // intake [Collider[]]
        yield return new CodeInstruction(OpCodes.Ldarg_1); // mode [Scp914Mode]
        yield return new CodeInstruction(OpCodes.Ldarg_2); // setting [Scp914KnobSetting]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Upgrade), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static void Invoke(Collider[] intake, Scp914Mode mode, Scp914KnobSetting setting)
    {
        if (!NetworkServer.active)
            throw new InvalidOperationException("Scp914Upgrade.Upgrade is a serverside-only script.");

        try
        {
            var hashSet = HashSetPool<GameObject>.Shared.Rent();

            Scp914UpgradeEvent ev = new([], [], mode, setting);

            foreach (Collider t in intake)
            {
                GameObject gameObject = t.transform.root.gameObject;

                if (!hashSet.Add(gameObject))
                    continue;

                if (ReferenceHub.TryGetHub(gameObject, out ReferenceHub? hub))
                {
                    Player? pl = hub.GetPlayer();

                    if (pl is null)
                        continue;

                    ev.Players.Add(pl);
                }
                else if (gameObject.TryGetComponent(out ItemPickupBase item))
                {
                    ev.Items.Add(item);
                }
            }

            ev.InvokeEvent();

            if (!ev.Allowed)
                return;

            mode = ev.Mode;
            setting = ev.Setting;

            bool upgradeDropped = (mode & Scp914Mode.Dropped) == Scp914Mode.Dropped;
            bool flag = (mode & Scp914Mode.Inventory) == Scp914Mode.Inventory;
            bool heldOnly = flag && (mode & Scp914Mode.Held) == Scp914Mode.Held;

            foreach (Player pl in ev.Players)
                Scp914Upgrader.ProcessPlayer(pl.ReferenceHub, flag, heldOnly, setting);
            foreach (ItemPickupBase item in ev.Items)
                Scp914Upgrader.ProcessPickup(item, upgradeDropped, setting);

            HashSetPool<GameObject>.Shared.Return(hashSet);
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <SCPs> {{Scp914}} [Upgrade]: {e}\n{e.StackTrace}");
        }
    }
}