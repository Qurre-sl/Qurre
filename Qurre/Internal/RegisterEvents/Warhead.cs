using LabApi.Events.Handlers;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.Attributes;
using Qurre.Internal.EventsManager;
using LabApiEvents = LabApi.Events.Arguments.WarheadEvents;

namespace Qurre.Internal.RegisterEvents;

internal static class Warhead
{
    [SelfInvoke]
    internal static void Init()
    {
        WarheadEvents.Starting += OnStart;
        WarheadEvents.Stopping += OnStop;
        WarheadEvents.Detonated += OnDetonate;
    }

    private static void OnStart(LabApiEvents.WarheadStartingEventArgs labApiEv)
    {
        var qurreEv = new AlphaStartEvent(labApiEv.Player.GetPlayer(), labApiEv.IsAutomatic, labApiEv.SuppressSubtitles, labApiEv.WarheadState);
        qurreEv.InvokeEvent();

        labApiEv.IsAutomatic = qurreEv.IsAutomatic;
        labApiEv.IsAllowed = qurreEv.IsAllowed;
        qurreEv.SuppressSubtitles = qurreEv.SuppressSubtitles;
        labApiEv.WarheadState = qurreEv.State;
    }

    private static void OnStop(LabApiEvents.WarheadStoppingEventArgs labApiEv)
    {
        var qurreEv = new AlphaStopEvent(labApiEv.Player.GetPlayer(), labApiEv.WarheadState);
        qurreEv.InvokeEvent();

        labApiEv.WarheadState = qurreEv.State;
        labApiEv.IsAllowed = qurreEv.IsAllowed;
    }

    private static void OnDetonate(LabApiEvents.WarheadDetonatedEventArgs labApiEv)
    {
        var qurreEv = new AlphaDetonateEvent();
        qurreEv.InvokeEvent();
    }
}