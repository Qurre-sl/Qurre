using HarmonyLib;
using InventorySystem.Items.Pickups;
using Mirror;
using NorthwoodLib.Pools;
using Scp914;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace Qurre.Internal.Patches.Scp.Scp914
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.Upgrade))]
    static class Upgrade
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0); // intake [Collider[]]
            yield return new CodeInstruction(OpCodes.Ldarg_1); // moveVector [Vector3]
            yield return new CodeInstruction(OpCodes.Ldarg_2); // mode [Scp914Mode]
            yield return new CodeInstruction(OpCodes.Ldarg_3); // setting [Scp914KnobSetting]
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Upgrade), nameof(Upgrade.Invoke)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        static void Invoke(Collider[] intake, Vector3 moveVector, Scp914Mode mode, Scp914KnobSetting setting)
        {
            if (!NetworkServer.active)
            {
                throw new InvalidOperationException("Scp914Upgrader.Upgrade is a serverside-only script.");
            }

            try
            {
                HashSet<GameObject> hashSet = HashSetPool<GameObject>.Shared.Rent();

                Scp914UpgradeEvent ev = new(new(), new(), moveVector, mode, setting);

                for (int i = 0; i < intake.Length; i++)
                {
                    GameObject gameObject = intake[i].transform.root.gameObject;

                    if (!hashSet.Add(gameObject))
                        continue;

                    if (ReferenceHub.TryGetHub(gameObject, out var hub))
                    {
                        Player pl = hub.GetPlayer();

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

                moveVector = ev.Move;
                mode = ev.Mode;
                setting = ev.Setting;

                bool upgradeDropped = (mode & Scp914Mode.Dropped) == Scp914Mode.Dropped;
                bool flag = (mode & Scp914Mode.Inventory) == Scp914Mode.Inventory;
                bool heldOnly = flag && (mode & Scp914Mode.Held) == Scp914Mode.Held;

                foreach (Player pl in ev.Players)
                {
                    Scp914Upgrader.ProcessPlayer(pl.ReferenceHub, flag, heldOnly, moveVector, setting);
                }
                foreach (ItemPickupBase item in ev.Items)
                {
                    Scp914Upgrader.ProcessPickup(item, upgradeDropped, moveVector, setting);
                }

                HashSetPool<GameObject>.Shared.Return(hashSet);
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <SCPs> {{Scp914}} [Upgrade]: {e}\n{e.StackTrace}");
            }
        }
    }
}