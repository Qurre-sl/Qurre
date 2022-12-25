using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using System;
namespace Qurre.Internal.Patches.Player
{
    [HarmonyPatch(typeof(ReservedSlot), nameof(ReservedSlot.HasReservedSlot))]
    static internal class ReserveSlot
    {
        [HarmonyPrefix]
        static bool Call(string userId, ref bool __result)
        {
            try
            {
                bool allow = ReservedSlot.Users.Contains(userId.Trim()) || !CharacterClassManager.OnlineMode;

                CheckReserveSlotEvent ev = new(userId, allow);
                ev.InvokeEvent();

                __result = ev.Allowed;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> [ReserveSlot]:{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}