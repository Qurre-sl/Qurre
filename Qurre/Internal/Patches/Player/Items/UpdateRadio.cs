using HarmonyLib;
using InventorySystem.Items.Radio;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Items
{
    using Qurre.API;
    using Qurre.API.Objects;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.ServerProcessCmd))]
    static class UpdateRadio
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0); // RadioItem
            yield return new CodeInstruction(OpCodes.Ldarg_1); // RadioCommand
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UpdateRadio), nameof(UpdateRadio.Invoke)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        static void Invoke(RadioItem instance, RadioMessages.RadioCommand command)
        {
            byte range = instance._rangeId;
            bool enabled = instance._enabled;

            switch (command)
            {
                case RadioMessages.RadioCommand.Enable:
                    if (instance._battery > 0f)
                        enabled = true;
                    break;

                case RadioMessages.RadioCommand.Disable:
                    enabled = false;
                    break;

                case RadioMessages.RadioCommand.ChangeRange:
                    {
                        byte b = (byte)(range + 1);
                        if (b >= instance.Ranges.Length)
                            b = 0;

                        range = b;
                        break;
                    }
            }

            UpdateRadioEvent ev = new(instance.Owner.GetPlayer(), instance, (RadioStatus)range, enabled);
            ev.InvokeEvent();

            if (!ev.Allowed) return;

            instance._rangeId = (byte)ev.Range;
            instance._enabled = ev.Enabled;

            instance.SendStatusMessage();
        }
    }
}