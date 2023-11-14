using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using PlayerStatsSystem;
using System.Linq;

namespace Qurre.Internal.Patches.Player.Health
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.DealDamage))]
    static class Damage
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new(instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Callvirt && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name == nameof(DamageHandlerBase.ApplyDamage));

            if (index < 3)
            {
                Log.Error($"Creating Patch error: <Player> {{Health}} [Damage]: Index - {index} < 3");
                return list.AsEnumerable();
            }

            Label contLabel = generator.DefineLabel();
            var _labels = list[index - 3].ExtractLabels();
            list[index - 3].labels.Add(contLabel);

            list.InsertRange(index - 3, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(_labels), // handler

                new CodeInstruction(OpCodes.Ldarg_0), // this
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PlayerStats), nameof(PlayerStats._hub))),

                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Damage), nameof(Damage.DamageCall))),
                new CodeInstruction(OpCodes.Brtrue, contLabel),

                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ret),
            });

            return list.AsEnumerable();
        }
        /*
         * ...
         * var @bool = Damage.DamageCall(handler, this._hub);
         * if (!@bool) return;
         * ...
        */

        static internal bool DamageCall(DamageHandlerBase handler, ReferenceHub hub)
        {
            try
            {
                var doDamage = GetDamage(handler);
                Player attacker = null;

                if (handler is AttackerDamageHandler adh)
                    attacker = adh.Attacker.Hub.GetPlayer();
                if (attacker is null)
                {
                    if (handler is ExplosionDamageHandler)
                        return false;
                    attacker = Server.Host;
                }

                DamageEvent ev = new(attacker, hub.GetPlayer(), handler, doDamage);

                if (ev.Target is null || ev.Target.Disconnected)
                    return true;

                ev.InvokeEvent();

                if (!SetDamage(handler, ev.Damage))
                    ev.Damage = doDamage;

                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Health}} [Damage]: {e}\n{e.StackTrace}");
                return true;
            }

            float GetDamage(DamageHandlerBase handler)
            {
                return handler switch
                {
                    StandardDamageHandler data => data.Damage,
                    _ => -1,
                };
            }
            bool SetDamage(DamageHandlerBase handler, float amout)
            {
                if (handler is StandardDamageHandler data) data.Damage = amout;
                else return false;
                return true;
            }
        }
    }
}