using HarmonyLib;
using VoiceChat.Playbacks;

namespace Qurre.Internal.Patches.Misc.Fixes
{
    [HarmonyPatch(typeof(PersonalRadioPlayback), nameof(PersonalRadioPlayback.IsTransmitting))]
    static class FixNullAudioFlood
    {
        [HarmonyPrefix]
        static bool Call(ReferenceHub hub) => hub is not null;
    }
}