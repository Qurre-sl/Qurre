using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CommandSystem;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using Qurre.Loader;
using RemoteAdmin;

namespace Qurre.Internal.Patches.Player.Admins
{
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), typeof(ReferenceHub), typeof(ICommandSender), typeof(string), typeof(long))]
    internal static class Ban
    {
        internal static bool CallEvent(ReferenceHub target, ICommandSender issuer, ref string reason, ref long duration)
        {
            try
            {
                if (target is null)
                {
                    return false;
                }

                API.Player issue = null;

                if (issuer is PlayerCommandSender plsender)
                {
                    issue = plsender.ReferenceHub.GetPlayer();
                }
                else if (issuer is CommandSender sender)
                {
                    issue = sender.GetPlayer();
                }

                BanEvent ev = new (target.GetPlayer(), issue ?? API.Server.Host, DateTime.Now.AddSeconds((uint)duration), reason);
                ev.InvokeEvent();

                reason = ev.Reason;
                duration = (long)(ev.Expires - DateTime.Now).TotalSeconds;

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Admins}} [Ban]:{e}\n{e.StackTrace}");
            }

            return true;
        }

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            List<CodeInstruction> list = new (instructions);

            int translate = list.FindLastIndex(ins => ins.opcode == OpCodes.Ldstr && ins.operand as string == "You have been banned. Reason: ");

            if (translate != -1)
            {
                list[translate] = new (OpCodes.Ldstr, Configs.Banned);
            }

            int index = list.FindLastIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase && methodBase.Name == nameof(BanPlayer.ApplyIpBan));

            if (index < 4)
            {
                Log.Error($"Creating Patch error: <Player> {{Admins}} [Ban]: Index - {index} < 4");
                return list.AsEnumerable();
            }

            list.InsertRange(
                index - 4, new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index - 4]),
                    new (OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarga_S, 2),
                    new CodeInstruction(OpCodes.Ldarga_S, 3),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Ban), nameof(CallEvent))),
                    new CodeInstruction(OpCodes.Brfalse, retLabel)
                });

            return list.AsEnumerable();
        }
    }
}