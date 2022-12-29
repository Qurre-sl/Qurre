using HarmonyLib;
using System;

namespace Qurre.Internal.Patches.Server
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(global::CheaterReport), nameof(global::CheaterReport.IssueReport))]
    static class CheaterReport
    {
        [HarmonyPrefix]
        static bool Call(string reporterUserId, string reportedUserId, string reason)
        {
            try
            {
                CheaterReportEvent ev = new(reporterUserId.GetPlayer(), reportedUserId.GetPlayer(), reason);
                ev.InvokeEvent();

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Server> [CheaterReport]:{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}