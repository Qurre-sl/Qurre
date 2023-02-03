using HarmonyLib;
using PlayerStatsSystem;
using Qurre.Events.Structs.SCPs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Qurre.Internal.Patches.SCPs.SCP_096
{
    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.AddTarget))]
    internal static class AddTargetLook
    {
        public static bool Prefix(PlayableScps.Scp096 __instance, GameObject target)
        {
            try
            {
                if (target == null) return true;
                API.Player targetPL = API.Player.Get(target);
                if (targetPL == null) return true;
                API.Player player = API.Player.Get(__instance.Hub.gameObject);
                var ev = new AddTargetEvent(__instance, player, targetPL);
                Qurre.Events.Invoke.Scp096.AddTarget(ev);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [AddTargetLook]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    // [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.OnDamage))]
    internal static class AddTargetShoot
    {
        public static bool Prefix(PlayableScps.Scp096 __instance, DamageHandlerBase handler)
        {
            try
            {
                if (handler is not AttackerDamageHandler dodo) return true;
                if (dodo.Attacker.Hub is null) return true;
                if (__instance.CanEnrage) return true;
                API.Player player = API.Player.Get(__instance.Hub.gameObject);
                API.Player target = API.Player.Get(dodo.Attacker.Hub);
                var ev = new AddTargetEvent(__instance, player, target);
                Qurre.Events.Invoke.Scp096.AddTarget(ev);
                __instance.AddTarget(dodo.Attacker.Hub.gameObject);
                __instance.Windup();
                __instance.Shield.SustainTime = 25f;
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [AddTargetShoot]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}
