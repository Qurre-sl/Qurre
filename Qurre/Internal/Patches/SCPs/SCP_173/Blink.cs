using HarmonyLib;
using Qurre.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Qurre.Internal.Patches.SCPs.SCP_173
{

    [HarmonyPatch(typeof(PlayerRoles.PlayableScps.Scp173.), nameof(PlayableScps.Scp173.ServerHandleBlinkMessage))]
    internal static class Blink
    {
        private static bool Prefix(PlayableScps.Scp173 __instance, ref Vector3 blinkPos)
        {
            try
            {
                List<Player> targets = __instance._observingPlayers.Select(x => Player.Get(x)).ToList();
                var ev = new BlinkEvent(Player.Get(__instance.Hub), blinkPos, targets);
                Qurre.Events.Invoke.Scp173.Blink(ev);
                blinkPos = ev.Position;
                if (ev.Allowed)
                {
                    __instance._observingPlayers.Clear();
                    foreach (var t in ev.Targets) try { if (t.ReferenceHub is not null) __instance._observingPlayers.Add(t.ReferenceHub); } catch { }
                }
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> Scp173 [Blink]:\n{e}\n{e.StackTrace}");
            }
            return false;
        }
    }
}
