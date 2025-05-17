using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CommandSystem;
using Footprinting;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using Qurre.Loader;
using RemoteAdmin;

namespace Qurre.Internal.Patches.PlayerEvents.Admins;

[HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), typeof(Footprint), typeof(ICommandSender), typeof(string),
    typeof(long))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Ban
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        List<CodeInstruction> list = [.. instructions];
        list.Last().labels.Add(retLabel);

        int translate = list.FindLastIndex(ins =>
            ins.opcode == OpCodes.Ldstr && ins.operand as string == "You have been banned. Reason: ");
        if (translate != -1)
            list[translate] = new CodeInstruction(OpCodes.Ldstr, Configs.Banned);

        int index = list.FindLastIndex(ins =>
            ins.opcode == OpCodes.Call && ins.operand is MethodBase { Name: nameof(BanPlayer.ApplyIpBan) });

        if (index < 4)
        {
            Log.Error($"Creating Patch error: <Player> {{Admins}} [Ban]: Index - {index} < 4");
            return list.AsEnumerable();
        }

        list.InsertRange(index - 4,
        [
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index - 4]),
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldarga_S, 2),
            new CodeInstruction(OpCodes.Ldarga_S, 3),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Ban), nameof(CallEvent))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        return list.AsEnumerable();
    }

    internal static bool CallEvent(Footprint footprint, ICommandSender issuer, ref string reason, ref long duration)
    {
        try
        {
            Player? target = footprint.Hub.GetPlayer();

            if (target is null)
                return false;

            Player? issue = issuer switch
            {
                PlayerCommandSender plSender => plSender.ReferenceHub.GetPlayer(),
                CommandSender sender => sender.GetPlayer(),
                _ => null
            };

            BanEvent ev = new(target, issue ?? Server.Host, DateTime.Now.AddSeconds((uint)duration),
                reason);
            ev.InvokeEvent();

            reason = ev.Reason;
            duration = (long)(ev.Expires - DateTime.Now).TotalSeconds;

            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Admins}} [Ban]: {e}\n{e.StackTrace}");
        }

        return true;
    }
}