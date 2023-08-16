using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using LiteNetLib.Utils;
using LiteNetLib;

namespace Qurre.Internal.Patches.Player.Network
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
    static class Preauth
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            List<CodeInstruction> list = new(instructions);

            int index = list.FindLastIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name == nameof(CustomLiteNetLib4MirrorTransport.ProcessCancellationData));

            List<Label> labels = list[index - 14].ExtractLabels();
            list.RemoveRange(index - 14, 15);
            list.InsertRange(index - 14, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(labels), // request
                new CodeInstruction(OpCodes.Ldloc_S, 10), // "text" (userid)

                new CodeInstruction(OpCodes.Ldloc_S, 28), // centralAuthPreauthFlags
                new CodeInstruction(OpCodes.Ldloc_S, 13), // text2 (region)

                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Preauth), nameof(Preauth.AuthCheck))),
                //new CodeInstruction(OpCodes.Brfalse, retLabel),
                // OpCodes.Brtrue
            });

            return list.AsEnumerable();
        }
        /*
         * ...
         * if(
         *  Preauth.AuthCheck(request, text, centralAuthPreauthFlags, text2) == true
         * ){
         * ...
         * }
         * ..
        */

        static internal bool AuthCheck(ConnectionRequest req, string userid, CentralAuthPreauthFlags flags, string region)
        {
            try
            {
                PreauthEvent ev = new(userid, req.RemoteEndPoint.Address, flags, region, req);
                ev.InvokeEvent();
                if (!ev.Allowed)
                {
                    if (CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
                        ServerConsole.AddLog($"Incoming connection from {req.RemoteEndPoint} rejected by a plugin.", ConsoleColor.Gray);

                    req.Reject(Generate(ev));
                }

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Network}} [Preauth]: {e}\n{e.StackTrace}");
            }

            return true;

            static NetDataWriter Generate(PreauthEvent ev)
            {
                try
                {
                    NetDataWriter netDataWriter = new();
                    netDataWriter.Put((byte)ev.RejectionReason);

                    if (ev.RejectionReason == RejectionReason.Banned)
                    {
                        netDataWriter.Put(ev.RejectionExpiration);
                        netDataWriter.Put(ev.RejectionCustomReason);
                    }
                    else if (ev.RejectionReason == RejectionReason.Custom)
                    {
                        netDataWriter.Put(ev.RejectionCustomReason);
                    }
                    else if (ev.RejectionReason == RejectionReason.Redirect)
                    {
                        netDataWriter.Put(ev.RejectionRedirectPort);
                    }
                    else if (ev.RejectionReason == RejectionReason.Delay)
                    {
                        netDataWriter.Put(ev.RejectionDelay);
                    }

                    return netDataWriter;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}