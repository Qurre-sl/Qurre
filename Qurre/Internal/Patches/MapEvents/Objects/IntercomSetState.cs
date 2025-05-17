using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerRoles.Voice;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.MapEvents.Objects;

[HarmonyPatch(typeof(Intercom), nameof(Intercom.State), MethodType.Setter)]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class IntercomSetState
{
    [HarmonyPrefix]
    private static bool Call(ref IntercomState value)
    {
        IntercomSetStateEvent @event = new(value);
        @event.InvokeEvent();

        value = @event.State;

        return @event.Allowed;
    }
}