using Hazards;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Qurre.API;
using Qurre.API.Attributes;
using Qurre.API.Controllers;
using Qurre.Events;

namespace Qurre.Internal.EventsCalled
{
    static class Round
    {
        [EventMethod(RoundEvents.Waiting)]
        static internal void Waiting()
        {
            Server.host = null;
            Server.hinv = null;

            API.Extensions.DamagesCached.Clear();
            Patches.Player.Admins.Banned.Cached.Clear();

            if (API.Round.CurrentRound == 0)
                API.Addons.Prefabs.InitLate();

            API.Round.CurrentRound++;

            RoundSummary.RoundLock = false;

            MapRoundInit();
        }

        static void MapRoundInit()
        {
            MapClearLists();

            Map.AmbientSoundPlayer = Server.Host.GameObject.GetComponent<AmbientSoundPlayer>();

            foreach (var room in RoomIdentifier.AllRoomIdentifiers)
            {
                Room _room = new(room);
                Map.Rooms.Add(_room);
                Map.Cameras.AddRange(_room.Cameras);
            }

            foreach (var door in Server.GetObjectsOf<DoorVariant>())
                Map.Doors.Add(new(door));

            foreach (var hole in Server.GetObjectsOf<SinkholeEnvironmentalHazard>())
                Map.Sinkholes.Add(new(hole));

            foreach (var tesla in Server.GetObjectsOf<TeslaGate>())
                Map.Teslas.Add(new(tesla));

            foreach (var window in Server.GetObjectsOf<BreakableWindow>())
                Map.Windows.Add(new(window));
        }

        static void MapClearLists()
        {
            foreach (var x in Map.Teslas)
            {
                if (x is null) continue;
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

            Patches.Player.Admins.Banned.Cached.Clear();

            Extensions.DamagesCached.Clear();

            try { API.Addons.Models.Model.ClearCache(); } catch { }

            Item.BaseToItem.Clear();
            Pickup.BaseToItem.Clear();
        }
    }
}