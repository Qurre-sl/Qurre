using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace Qurre.Internal.Patches.Map.Objects
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    static class TriggerTesla
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0); // instance [TeslaGateController]
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TriggerTesla), nameof(TriggerTesla.Invoke)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        static void Invoke(TeslaGateController instance)
        {
            try
            {
                if (instance is null)
                    return;

                if (!NetworkServer.active)
                {
                    foreach (TeslaGate teslaGate2 in instance.TeslaGates)
                        teslaGate2.ClientSideCode();
                    return;
                }

                var players = Player.List;

                foreach (TeslaGate teslaGate in instance.TeslaGates)
                {
                    if (!teslaGate.isActiveAndEnabled)
                        continue;

                    if (teslaGate.InactiveTime > 0f)
                    {
                        teslaGate.NetworkInactiveTime = Mathf.Max(0f, teslaGate.InactiveTime - Time.fixedDeltaTime);
                        continue;
                    }

                    var tesla = teslaGate.GetTesla();
                    if (!tesla.Enable)
                        continue;

                    bool idling = false;
                    bool activated = false;

                    foreach (Player pl in players)
                    {
                        if (pl.IsHost)
                            continue;

                        if (!pl.RoleInfomation.IsAlive)
                            continue;

                        bool inidl = teslaGate.PlayerInIdleRange(pl.ReferenceHub);
                        if (!inidl)
                            continue;

                        if (pl.Invisible)
                            continue;

                        bool inrng = teslaGate.PlayerInRange(pl.ReferenceHub);

                        TriggerTeslaEvent ev = new(pl, tesla, inidl, inrng);
                        ev.InvokeEvent();

                        if (!ev.Allowed)
                            continue;

                        if (!idling)
                            idling = true;

                        if (!activated && inrng && !teslaGate.InProgress)
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
}