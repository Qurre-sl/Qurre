using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Network;

[HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport),
    nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Preauth
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        List<CodeInstruction> list = [..instructions];
        list.Last().labels.Add(retLabel);

        int index = list.FindLastIndex(ins =>
            ins.opcode == OpCodes.Call && ins.operand is MethodBase { Name: "ProcessCancellationData" });

        var labels = list[index - 14].ExtractLabels();
        list.RemoveRange(index - 14, 15);
        list.InsertRange(index - 14,
        [
            new CodeInstruction(OpCodes.Ldarg_1).WithLabels(labels), // request
            new CodeInstruction(OpCodes.Ldloc_S, 10), // "text" (userid)

            new CodeInstruction(OpCodes.Ldloc_S, 28), // centralAuthPreauthFlags
            new CodeInstruction(OpCodes.Ldloc_S, 13), // text2 (region)

            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Preauth), nameof(AuthCheck)))
            //new(OpCodes.Brfalse, retLabel),
            // OpCodes.Brtrue
        ]);

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

    internal static bool AuthCheck(ConnectionRequest req, string userid, CentralAuthPreauthFlags flags, string region)
    {
        try
        {
            PreauthEvent ev = new(userid, req.RemoteEndPoint.Address, flags, region, req);
            ev.InvokeEvent();

            if (ev.Allowed)
                return true;

            if (CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
                ServerConsole.AddLog($"Incoming connection from {req.RemoteEndPoint} rejected by a plugin.");

            req.Reject(Generate(ev));

            return false;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Network}} [Preauth]: {e}\n{e.StackTrace}");
        }

        return true;

        static NetDataWriter? Generate(PreauthEvent ev)
        {
            try
            {
                NetDataWriter netDataWriter = new();
                netDataWriter.Put((byte)ev.RejectionReason);

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (ev.RejectionReason)
                {
                    case RejectionReason.Banned:
                    {
                        netDataWriter.Put(ev.RejectionExpiration);
                        netDataWriter.Put(ev.RejectionCustomReason);
                        break;
                    }
                    case RejectionReason.Custom:
                    {
                        netDataWriter.Put(ev.RejectionCustomReason);
                        break;
                    }
                    case RejectionReason.Redirect:
                    {
                        netDataWriter.Put(ev.RejectionRedirectPort);
                        break;
                    }
                    case RejectionReason.Delay:
                    {
                        netDataWriter.Put(ev.RejectionDelay);
                        break;
                    }
                }

                return netDataWriter;
            }
            catch
            {
                return null;
            }
        } // end void Generate
    } // end bool AuthCheck
}