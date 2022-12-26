using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;
using LiteNetLib;

namespace Qurre.Internal.Patches.Player
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
            var found = false;

            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            foreach (var ins in instructions)
            {
                yield return ins;
                if (!found)
                {
                    if (ins.opcode == OpCodes.Stloc_S && ins.operand is not null && ins.operand is LocalBuilder localBuilder &&
                        localBuilder.LocalIndex == 30)
                    {
                        found = true;

                        Log.Info("found");

                        yield return new CodeInstruction(OpCodes.Ldarg_1); // request
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 10); // "text" (userid)

                        yield return new CodeInstruction(OpCodes.Ldloc_S, 28); // centralAuthPreauthFlags
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 13); // text2 (region)
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 30); // netPeer (peer)

                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Preauth), nameof(Preauth.AuthCheck)));
                        yield return new CodeInstruction(OpCodes.Brfalse, retLabel);
                    }
                }
            }
        }

        static internal bool AuthCheck(ConnectionRequest req, string userid, CentralAuthPreauthFlags flags, string region, NetPeer peer)
        {
            try
            {
                PreauthEvent ev = new(userid, req.RemoteEndPoint.Address, flags, region, req);
                ev.InvokeEvent();
                if (!ev.Allowed)
                {
                    if (CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
                        ServerConsole.AddLog($"Incoming connection from {req.RemoteEndPoint} rejected by a plugin.", ConsoleColor.Gray);

                    peer.Disconnect();
                    req.Reject();
                }

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> [Preauth]:{e}\n{e.StackTrace}");
            }

            return true;
        }
    }
}