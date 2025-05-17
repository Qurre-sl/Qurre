using LabApi.Events.Handlers;
using Qurre.Events.Structs;
using Qurre.Internal.Attributes;
using Qurre.Internal.EventsManager;
using UnityEngine;
using LabEvents = LabApi.Events.Arguments.ServerEvents;

namespace Qurre.Internal.RegisterEvents;

internal static class Round
{
    [SelfInvoke]
    internal static void Init()
    {
        ServerEvents.WaitingForPlayers += OnWaiting;
        ServerEvents.RoundRestarted += OnRestarted;
        ServerEvents.RoundStarted += OnStart;
    }

    private static void OnWaiting()
    {
        Debug.Log("[Qurre] Waiting for players...");
        new WaitingEvent().InvokeEvent();
    }

    private static void OnRestarted()
    {
        new RoundRestartEvent().InvokeEvent();
    }

    private static void OnStart() // LabEvents.RoundEndingEventArgs ev
    {
        new RoundStartedEvent().InvokeEvent();
    }
}