using System;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Network
{
    [HarmonyPatch(typeof(ReservedSlot), nameof(ReservedSlot.HasReservedSlot))]
    internal static class ReserveSlot
    {
        [HarmonyPrefix]
        private static bool Call(string userId, ref bool __result)
        {
            try
            {
                bool allow = ReservedSlot.Users.Contains(userId.Trim()) || !CharacterClassManager.OnlineMode;

                CheckReserveSlotEvent ev = new (userId, allow);
                ev.InvokeEvent();

                __result = ev.Allowed;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Network}} [ReserveSlot]:{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}