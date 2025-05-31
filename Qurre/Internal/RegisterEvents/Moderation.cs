using System;
using LabApi.Events.Arguments.Scp096Events;
using LabApi.Events.Arguments.Scp173Events;
using LabApi.Events.Handlers;
using LabApi.Features.Extensions;
using MapGeneration.Distributors;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using Qurre.API.World;
using Qurre.Events.Structs;
using Qurre.Internal.Attributes;
using Qurre.Internal.EventsManager;
using UnityEngine;
using LabEvents = LabApi.Events.Arguments.PlayerEvents;
using Locker = MapGeneration.Distributors.Locker;

namespace Qurre.Internal.RegisterEvents;

internal static class Moderation
{
    [SelfInvoke]
    internal static void Init()
    {
        SpawnableStructure.OnAdded += OnRoomInit;
        PlayerEvents.PreAuthenticating += OnPreAuth;
        PlayerEvents.Joined += OnJoin;
        PlayerEvents.ChangedSpectator += OnChangedSpectator;
        PlayerEvents.ChangingRole += OnChangeRole;
        PlayerEvents.Spawning += OnSpawning;
        PlayerEvents.UpdatingEffect += OnUpdatingEffect;
        PlayerEvents.InteractingDoor += OnInteractingDoor;
        PlayerEvents.PickingUpItem += OnPickupItem;
        PlayerEvents.PickingUpArmor += OnPickupArmor;
        PlayerEvents.PickingUpAmmo += OnPickupAmmo;
        PlayerEvents.UsedItem += OnUsedItem;
        PlayerEvents.SearchedPickup += OnSearch;
        PlayerEvents.Escaping += OnEscaping;
        PlayerEvents.ReportingPlayer += OnLocalReport;
        Scp173Events.AddingObserver += OnAddObserver;
        Scp173Events.RemovedObserver += OnRemovedObserver;
        Scp173Events.BreakneckSpeedChanging += OnEnableSpeed;
        Scp096Events.AddingTarget += OnAddingTarget;
        Scp096Events.Charging += OnCharging;
        Scp096Events.StartCrying += OnStartCrying;
        Scp096Events.TryingNotToCry += OnTryingNotToCry;
        Scp096Events.Enraging += OnEnraging;
        Scp096Events.ChangingState += OnChangingState;
    }

    private static void OnRoomInit(SpawnableStructure ev)
    {
        try
        {
            if (ev is not Locker locker)
                return;

            Map.Lockers.RemoveAll(x => !x.GameObject);
            Map.Lockers.Add(new API.Controllers.Locker(locker));
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Misc> {{Creates}} [Locker]: {e}\n{e.StackTrace}");
        }
    }

    private static void OnPreAuth(LabEvents.PlayerPreAuthenticatingEventArgs ev)
    {
        PreauthEvent rep = new(ev.UserId, ev.IpAddress, ev.Flags, ev.Region, ev.ConnectionRequest);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnJoin(LabEvents.PlayerJoinedEventArgs ev)
    {
        new JoinEvent(new Player(ev.Player.ReferenceHub)).InvokeEvent();
    }

    private static void OnChangedSpectator(LabEvents.PlayerChangedSpectatorEventArgs ev)
    {
        new ChangeSpectateEvent(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.OldTarget.GetPlayer(),
            ev.NewTarget.GetPlayer()).InvokeEvent();
    }

    private static void OnChangeRole(LabEvents.PlayerChangingRoleEventArgs ev)
    {
        if (!ev.NewRole.IsDead())
            return;

        SpawnEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.NewRole, Vector3.zero, Vector3.zero);
        rep.InvokeEvent();
    }

    private static void OnSpawning(LabEvents.PlayerSpawningEventArgs ev)
    {
        SpawnEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.Role.RoleTypeId, ev.SpawnLocation, new Vector3(0, ev.HorizontalRotation));
        rep.InvokeEvent();
        ev.SpawnLocation = rep.Position;
        ev.HorizontalRotation = rep.Rotation.y;
    }

    private static void OnUpdatingEffect(LabEvents.PlayerEffectUpdatingEventArgs ev)
    {
        switch (ev)
        {
            case { Intensity: > 0, Effect.Intensity: 0 }:
                {
                    EffectEnabledEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
                        ev.Effect);
                    rep.InvokeEvent();
                    ev.IsAllowed = rep.Allowed;
                    break;
                }
            case { Effect.Intensity: > 0, Intensity: 0 }:
                {
                    EffectDisabledEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
                        ev.Effect);
                    rep.InvokeEvent();
                    ev.IsAllowed = rep.Allowed;
                    break;
                }
        }
    }

    private static void OnInteractingDoor(LabEvents.PlayerInteractingDoorEventArgs ev)
    {
        InteractDoorEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.Door.Base.GetDoor(), ev.CanOpen);
        rep.InvokeEvent();
        ev.CanOpen = rep.Allowed;
    }

    private static void OnPickupItem(LabEvents.PlayerPickingUpItemEventArgs ev)
    {
        PickupItemEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.Pickup.Base);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnPickupArmor(LabEvents.PlayerPickingUpArmorEventArgs ev)
    {
        PickupArmorEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.BodyArmorPickup.Base);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnPickupAmmo(LabEvents.PlayerPickingUpAmmoEventArgs ev)
    {
        PickupAmmoEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.AmmoPickup.Base);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnUsedItem(LabEvents.PlayerUsedItemEventArgs ev)
    {
        UsedItemEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.UsableItem.Base);
        rep.InvokeEvent();
    }

    private static void OnSearch(LabEvents.PlayerSearchedPickupEventArgs ev)
    {
        PrePickupItemEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.Pickup.Base);
        rep.InvokeEvent();
    }

    private static void OnEscaping(LabEvents.PlayerEscapingEventArgs ev)
    {
        EscapeEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(), ev.NewRole)
        {
            Allowed = ev.IsAllowed && ev.EscapeScenario != Escape.EscapeScenarioType.None
        };
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnLocalReport(LabEvents.PlayerReportingPlayerEventArgs ev)
    {
        LocalReportEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.Target.GetPlayer() ?? throw new NullReferenceException(), ev.Reason);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnAddObserver(Scp173AddingObserverEventArgs ev)
    {
        Scp173AddObserverEvent rep = new(ev.Target.GetPlayer() ?? throw new NullReferenceException(),
            ev.Player.GetPlayer() ?? throw new NullReferenceException());
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnRemovedObserver(Scp173RemovedObserverEventArgs ev)
    {
        Scp173RemovedObserverEvent rep = new(ev.Target.GetPlayer() ?? throw new NullReferenceException(),
            ev.Player.GetPlayer() ?? throw new NullReferenceException());
        rep.InvokeEvent();
    }

    private static void OnEnableSpeed(Scp173BreakneckSpeedChangingEventArgs ev)
    {
        Scp173EnableSpeedEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.Active);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnAddingTarget(Scp096AddingTargetEventArgs ev)
    {
        // ReferenceHub scp, ReferenceHub target, bool isLooking
        Scp096AddTargetEvent rep = new(ev.Player.ReferenceHub, ev.Target.ReferenceHub, ev.WasLooking);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnCharging(Scp096ChargingEventArgs ev)
    {
        Scp096SetStateEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            Scp096State.Charging);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnStartCrying(Scp096StartCryingEventArgs ev)
    {
        Scp096SetStateEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            Scp096State.StartCrying);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnTryingNotToCry(Scp096TryingNotToCryEventArgs ev)
    {
        Scp096SetStateEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            Scp096State.TryNotCry);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnEnraging(Scp096EnragingEventArgs ev)
    {
        Scp096SetStateEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            Scp096State.Enraging);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }

    private static void OnChangingState(Scp096ChangingStateEventArgs ev)
    {
        Scp096SetStateEvent rep = new(ev.Player.GetPlayer() ?? throw new NullReferenceException(),
            ev.State);
        rep.InvokeEvent();
        ev.IsAllowed = rep.Allowed;
    }
}