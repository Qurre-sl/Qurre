using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using UnityEngine;

namespace Qurre.Internal.Patches.MapEvents.Objects;

[HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class TriggerTesla
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // instance [TeslaGateController]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TriggerTesla), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static void Invoke(TeslaGateController instance)
    {
        try
        {
            if (instance == null)
                return;

            if (!NetworkServer.active)
            {
                foreach (TeslaGate teslaGate2 in TeslaGate.AllGates)
                    teslaGate2.ClientSideCode();
                return;
            }

            List<Player> players = [.. Player.List.Where(x => !x.IsHost && x.RoleInformation.IsAlive)];

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (TeslaGate teslaGate in TeslaGate.AllGates)
            {
                if (!teslaGate.isActiveAndEnabled)
                    continue;

                if (teslaGate.InactiveTime > 0f)
                {
                    teslaGate.NetworkInactiveTime = Mathf.Max(0f, teslaGate.InactiveTime - Time.fixedDeltaTime);
                    continue;
                }

                Tesla tesla = teslaGate.GetTesla();
                if (!tesla.Enable)
                    continue;

                bool idling = false;
                bool activated = false;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (Player pl in players)
                {
                    bool inIdle = teslaGate.IsInIdleRange(pl.ReferenceHub);
                    if (!inIdle)
                        continue;

                    bool inRng = teslaGate.PlayerInRange(pl.ReferenceHub);

                    TriggerTeslaEvent ev = new(pl, tesla, inIdle, inRng);
                    ev.InvokeEvent();

                    if (!ev.Allowed)
                        continue;

                    if (!idling)
                        idling = true;

                    if (!activated && ev.InRageRange && !teslaGate.InProgress)
                        activated = true;
                }

                if (activated)
                    teslaGate.ServerSideCode();
                if (idling != teslaGate.isIdling)
                    teslaGate.ServerSideIdle(activated);
            }
        }
        catch (NullReferenceException)
        {
            //Debug.Log(e);
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Map> {{Objects}} [TriggerTesla]: {e}\n{e.StackTrace}");
        }
    }
}