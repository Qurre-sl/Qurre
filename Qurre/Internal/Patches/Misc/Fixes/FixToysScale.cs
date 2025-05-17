using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using AdminToys;
using HarmonyLib;

namespace Qurre.Internal.Patches.Misc.Fixes;

[HarmonyPatch(typeof(AdminToyBase), nameof(AdminToyBase.UpdatePositionServer))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class FixToysScale
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // instance [AdminToyBase]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FixToysScale), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static void Invoke(AdminToyBase instance)
    {
        if (instance.IsStatic)
            return;

        if (instance.transform == null)
            return;

        instance.NetworkPosition = instance.transform.position;
        instance.NetworkRotation = instance.transform.rotation;
        instance.NetworkScale = instance.transform.lossyScale;
    }
}