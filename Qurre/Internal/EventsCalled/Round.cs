﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Hazards;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using MapGeneration;
using MEC;
using Qurre.API;
using Qurre.API.Addons;
using Qurre.API.Addons.Audio;
using Qurre.API.Addons.Models;
using Qurre.API.Attributes;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Structs;
using Qurre.Events;
using Qurre.Internal.Patches.PlayerEvents.Admins;
using Scp914;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Qurre.Internal.EventsCalled;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Round
{
    static Round()
    {
        SceneManager.sceneUnloaded += SceneUnloaded;
    }

    [EventMethod(RoundEvents.Start)]
    private static void Started()
    {
        API.Round.LocalStarted = true;
        API.Round.LocalWaiting = false;
    }

    [EventMethod(RoundEvents.Restart)]
    private static void DestroyAudio()
    {
        API.Audio.LocalHostAudioPlayer = null;

        foreach (AudioPlayer? player in AudioPlayer.Players.ToList())
            player.DestroyPlayer();

        AudioPlayer.Players.Clear();
    }

    [EventMethod(RoundEvents.Waiting)]
    private static void Waiting()
    {
        Server.WaitingRefresh();

        API.Round.LocalStarted = false;
        API.Round.ForceEnd = false;
        API.Round.LocalWaiting = true;
        API.Round.ActiveGenerators = 0;

        BcComponent.Refresh();

        Extensions.DamagesCached.Clear();
        Banned.Cached.Clear();

        if (API.Round.CurrentRound == 0)
            Prefabs.InitLate();

        API.Round.CurrentRound++;

        RoundSummary.RoundLock = false;

        MapRoundInit();
    }

    private static void MapRoundInit()
    {
        Map.AmbientSoundPlayer = Server.Host.GameObject.GetComponent<AmbientSoundPlayer>();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (RoomIdentifier? roomIdent in RoomIdentifier.AllRoomIdentifiers)
        {
            Room room = new(roomIdent);
            Map.Rooms.Add(room);
            Map.Cameras.AddRange(room.Cameras);
        }

        foreach (DoorVariant? door in Server.GetObjectsOf<DoorVariant>())
            Map.Doors.Add(new Door(door));

        foreach (SinkholeEnvironmentalHazard? hole in Server.GetObjectsOf<SinkholeEnvironmentalHazard>())
            Map.Sinkholes.Add(new Sinkhole(hole));

        foreach (TeslaGate? tesla in Server.GetObjectsOf<TeslaGate>())
            Map.Teslas.Add(new Tesla(tesla));

        foreach (BreakableWindow? window in Server.GetObjectsOf<BreakableWindow>())
            Map.Windows.Add(new Window(window));

        foreach (WorkstationController? station in WorkstationController.AllWorkstations)
            Map.WorkStations.Add(new WorkStation(station));


        API.Controllers.Scp914.Controller = Object.FindObjectOfType<Scp914Controller>();


        List<Door> updateDoors = [.. Map.Doors];

        UpdateDoors();
        return;

        void UpdateDoors()
        {
            List<Door> updates = [.. updateDoors];

            foreach (Door? door in updates)
                try
                {
                    foreach (Room? room in door.Rooms)
                        room.Doors.Add(door);
                    updateDoors.Remove(door);
                }
                catch
                {
                    // ignored
                }

            updates.Clear();

            if (updateDoors.Count == 0)
                return;

            Timing.CallDelayed(0.5f, UpdateDoors);
        }
    }

    private static void MapClearLists()
    {
        foreach (Tesla? x in Map.Teslas.OfType<Tesla>())
        {
            x.ImmunityRoles.Clear();
            x.ImmunityPlayers.Clear();
        }

        Map.Cassies.Clear();

        Map.Lights.Clear();
        Map.Primitives.Clear();
        Map.ShootingTargets.Clear();
        Map.WorkStations.Clear();

        Map.Cameras.Clear();
        Map.Doors.Clear();
        Map.Generators.Clear();
        Map.Lifts.Clear();
        Map.Lockers.Clear();
        Map.Ragdolls.Clear();
        Map.Rooms.Clear();
        Map.Sinkholes.Clear();
        Map.Teslas.Clear();
        Map.Windows.Clear();

        Room.NetworkIdentities.Clear();

        Banned.Cached.Clear();

        Extensions.DamagesCached.Clear();

        Item.BaseToItem.Clear();
        Pickup.BaseToItem.Clear();

        Model.ClearCache();
    }

    private static void SceneUnloaded(Scene _)
    {
        Fields.Player.Ids.Clear();
        Fields.Player.Args.Clear();
        Fields.Player.Hubs.Clear();
        Fields.Player.Dictionary.Clear();
        MapClearLists();
    }
}