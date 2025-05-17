using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using VoiceChat.Playbacks;

namespace Qurre.Internal.Patches.Misc.Fixes;

[HarmonyPatch(typeof(PersonalRadioPlayback), nameof(PersonalRadioPlayback.IsTransmitting))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class FixNullAudioFlood
{
    [HarmonyPrefix]
    private static bool Call(ReferenceHub? hub)
    {
        return hub != null;
    }
}