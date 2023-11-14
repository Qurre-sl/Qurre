using AdminToys;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Misc.Fixes
{
    [HarmonyPatch(typeof(AdminToyBase), nameof(AdminToyBase.UpdatePositionServer))]
    static class FixToysScale
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0); // instance [AdminToyBase]
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FixToysScale), nameof(FixToysScale.Invoke)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        static void Invoke(AdminToyBase instance)
        {
            try
            {
                instance.NetworkPosition = instance.transform.position;
                instance.NetworkRotation = new LowPrecisionQuaternion(instance.transform.rotation);
                instance.NetworkScale = instance.transform.lossyScale;
            }
            catch { }
        }
    }
}