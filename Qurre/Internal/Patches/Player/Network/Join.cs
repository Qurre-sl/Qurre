using CentralAuth;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;

namespace Qurre.Internal.Patches.Player.Network
{
    using Qurre.API;
    using Qurre.Events.Structs;

    [HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.ProcessAuthenticationResponse))]
    static class Join
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new(instructions);

            int index = 559; // mne len' delat' privazky k methody

            list.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index]), // this
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager._hub))),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Player))[1]),

                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(JoinEvent))[0]),

                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),
            });

            return list.AsEnumerable();
        }
        /*
         * ...
         * Qurre.Internal.EventsManager.Loader.InvokeEvent(new Qurre.Events.Structs.JoinEvent(new Qurre.API.Player(this._hub)));
         * ...
        */
    }
}