using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Radio;
using Qurre.API;
using Qurre.API.Objects;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Items
{
    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.ServerProcessCmd))]
    internal static class UpdateRadio
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            yield return new (OpCodes.Ldarg_0); // RadioItem
            yield return new (OpCodes.Ldarg_1); // RadioCommand
            yield return new (OpCodes.Call, AccessTools.Method(typeof(UpdateRadio), nameof(Invoke)));
            yield return new (OpCodes.Ret);
        }

        private static void Invoke(RadioItem instance, RadioMessages.RadioCommand command)
        {
            byte range = instance._rangeId;
            bool enabled = instance._enabled;

            switch (command)
            {
                case RadioMessages.RadioCommand.Enable:
                    if (instance._battery > 0f)
                    {
                        enabled = true;
                    }

                    break;

                case RadioMessages.RadioCommand.Disable:
                    enabled = false;
                    break;

                case RadioMessages.RadioCommand.ChangeRange:
                {
                    var b = (byte)(range + 1);

                    if (b >= instance.Ranges.Length)
                    {
                        b = 0;
                    }

                    range = b;
                    break;
                }
            }

            UpdateRadioEvent ev = new (instance.Owner.GetPlayer(), instance, (RadioStatus)range, enabled);
            ev.InvokeEvent();

            if (!ev.Allowed)
            {
                return;
            }

            instance._rangeId = (byte)ev.Range;
            instance._enabled = ev.Enabled;

            instance.SendStatusMessage();
        }
    }
}