using Hazards;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using MapGeneration;
using Qurre.API;
using Qurre.API.Addons;
using Qurre.API.Addons.Models;
using Qurre.API.Attributes;
using Qurre.API.Controllers;
using Qurre.Events;
using Qurre.Internal.Patches.Player.Admins;
using Scp914;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Qurre.Internal.EventsCalled
{
    internal static class Round
    {
        static Round()
            => SceneManager.sceneUnloaded += SceneUnloaded;

        [EventMethod(RoundEvents.Waiting)]
        internal static void Waiting()
        {
            Server.host = null;
            Server.hinv = null;

            Extensions.DamagesCached.Clear();
            Banned.Cached.Clear();

            if (API.Round.CurrentRound == 0)
            {
                Prefabs.InitLate();
            }

            API.Round.CurrentRound++;

            RoundSummary.RoundLock = false;

            MapRoundInit();
        }

        private static void MapRoundInit()
        {
            Map.AmbientSoundPlayer = Server.Host.GameObject.GetComponent<AmbientSoundPlayer>();

            foreach (RoomIdentifier room in RoomIdentifier.AllRoomIdentifiers)
            {
                Room _room = new (room);
                Map.Rooms.Add(_room);
                Map.Cameras.AddRange(_room.Cameras);
            }

            foreach (DoorVariant door in Server.GetObjectsOf<DoorVariant>())
            {
                Map.Doors.Add(new (door));
            }

            foreach (SinkholeEnvironmentalHazard hole in Server.GetObjectsOf<SinkholeEnvironmentalHazard>())
            {
                Map.Sinkholes.Add(new (hole));
            }

            foreach (TeslaGate tesla in Server.GetObjectsOf<TeslaGate>())
            {
                Map.Teslas.Add(new (tesla));
            }

            foreach (BreakableWindow window in Server.GetObjectsOf<BreakableWindow>())
            {
                Map.Windows.Add(new (window));
            }

            foreach (WorkstationController station in WorkstationController.AllWorkstations)
            {
                Map.WorkStations.Add(new (station));
            }


            foreach (Door door in Map.Doors)
            {
                foreach (Room room in door.Rooms)
                {
                    room.Doors.Add(door);
                }
            }


            API.Controllers.Scp914.Controller = Object.FindObjectOfType<Scp914Controller>();
        }

        private static void MapClearLists()
        {
            foreach (Tesla x in Map.Teslas)
            {
                if (x is null)
                {
                    continue;
                }

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

            try
            {
                Model.ClearCache();
            }
            catch { }

            Item.BaseToItem.Clear();
            Pickup.BaseToItem.Clear();
        }

        private static void SceneUnloaded(Scene _)
        {
            MapClearLists();
            Fields.Player.IDs.Clear();
            Fields.Player.UserIDs.Clear();
            Fields.Player.Args.Clear();
            Fields.Player.Dictionary.Clear();
        }
    }
}