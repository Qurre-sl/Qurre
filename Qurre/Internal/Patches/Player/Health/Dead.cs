using System;
using HarmonyLib;
using PlayerRoles;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Objects;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Health
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
    internal static class Dead
    {
        [HarmonyPrefix]
        private static bool CallPre(PlayerStats __instance, DamageHandlerBase handler)
        {
            try
            {
                DiesEvent ev = new (handler.GetAttacker(), __instance.gameObject.GetPlayer(), handler);
                ev.InvokeEvent();

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Health}} [Dies]:{e}\n{e.StackTrace}");
            }

            return true;
        }

        [HarmonyPostfix]
        private static void Call(PlayerStats __instance, DamageHandlerBase handler)
        {
            try
            {
                API.Player attacker = handler.GetAttacker();
                API.Player target = __instance.gameObject.GetPlayer();

                if (attacker is null)
                {
                    attacker = target;
                }

                if (target is not null && (target.RoleInfomation.Role != RoleTypeId.Spectator || target.GamePlay.GodMode || target.IsHost) || attacker is null)
                {
                    return;
                }

                DamageTypes type = handler.GetDamageType();
                var ev = new DeadEvent(attacker, target, handler, type);
                ev.InvokeEvent();

                if (attacker != target && attacker is not null && target is not null)
                {
                    attacker.PlayerStatsInfomation._kills.Add(new (attacker, target, type, DateTime.Now));
                    target.PlayerStatsInfomation.DeathsCount++;
                }
                //if (target.Bot && API.Map.Bots.TryFind(out var _bot, x => x.Player == target)) _bot.Destroy();
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Health}} [Dead]:{e}\n{e.StackTrace}");
            }
        }
    }
}