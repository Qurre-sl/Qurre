using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.ServerEvents;

[HarmonyPatch(typeof(global::CheaterReport), nameof(global::CheaterReport.IssueReport))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class CheaterReport
{
    [HarmonyPrefix]
    private static bool Call(string reporterUserId, string reportedUserId, string reason)
    {
        try
        {
            Player? issuer = reporterUserId.GetPlayer();
            Player? target = reportedUserId.GetPlayer();

            if (issuer is null || target is null)
                return false;

            CheaterReportEvent ev = new(issuer, target, reason);
            ev.InvokeEvent();

            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Server> [CheaterReport]: {e}\n{e.StackTrace}");
            return true;
        }
    }
}