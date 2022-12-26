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
            var found = false;

            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            foreach (var ins in instructions)
            {
                if (!found)
                {
                    if (ins.opcode == OpCodes.Callvirt && ins.operand is not null && ins.operand is MethodBase methodBase &&
                        methodBase.Name == nameof(DamageHandlerBase.ApplyDamage))
                    {
                        found = true;

                        // before: handler, this._hub
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Damage), nameof(Damage.DamageCall)));
                        yield return new CodeInstruction(OpCodes.Brfalse, retLabel);

                        yield return new CodeInstruction(OpCodes.Ldarg_1);

                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PlayerStats), nameof(PlayerStats._hub)));
                    }
                }
                yield return ins;
            }
        }
        /*
         * ...
         * var @bool = Damage.DamageCall(handler, this._hub);
         * if (!@bool) return;
         * ...
        */

        static internal bool DamageCall(DamageHandlerBase handler, ReferenceHub hub)
        {
            var doDamage = GetDamage(handler);
            Player attacker = null;

            if (handler is AttackerDamageHandler adh) attacker = adh.Attacker.Hub.GetPlayer();
            if (attacker is null) attacker = Server.Host;

            DamageEvent ev = new(attacker, hub.GetPlayer(), handler, doDamage);
            ev.InvokeEvent();

            if (!SetDamage(handler, ev.Damage)) ev.Damage = doDamage;

            return ev.Allowed;

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