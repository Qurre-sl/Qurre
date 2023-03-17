using HarmonyLib;
using System;
using PlayerRoles;
using PlayerStatsSystem;

namespace Qurre.Internal.Patches.Player.Health
{
    using Qurre.API;
    using Qurre.API.Addons;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
    static class Dead
    {
        [HarmonyPrefix]
        static bool CallPre(PlayerStats __instance, DamageHandlerBase handler)
        {
            try
            {
                DiesEvent ev = new(handler.GetAttacker(), __instance.gameObject.GetPlayer(), handler);

                if (ev.Target is null)
                    return true;

                ev.InvokeEvent();

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Health}} [Dies]: {e}\n{e.StackTrace}");
            }
            return true;
        }

        [HarmonyPostfix]
        static void Call(PlayerStats __instance, DamageHandlerBase handler)
        {
            try
            {
                Player attacker = handler.GetAttacker();
                Player target = __instance.gameObject.GetPlayer();

                if (attacker is null) attacker = target;
                if ((target is not null && (target.RoleInfomation.Role != RoleTypeId.Spectator ||
                    target.GamePlay.GodMode || target.IsHost)) || attacker is null) return;

                var type = handler.GetDamageType();
                var ev = new DeadEvent(attacker, target, handler, type);
                ev.InvokeEvent();

                if (attacker != target && attacker is not null && target is not null)
                {
                    attacker.PlayerStatsInfomation._kills.Add(new KillElement(attacker, target, type, DateTime.Now));
                    target.PlayerStatsInfomation.DeathsCount++;
                }
                //if (target.Bot && API.Map.Bots.TryFind(out var _bot, x => x.Player == target)) _bot.Destroy();
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Health}} [Dead]: {e}\n{e.StackTrace}");
            }
        }
    }
}