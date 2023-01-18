using System.Collections.Generic;
using InventorySystem.Items.Pickups;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Structs;
using UnityEngine;
using Camera = Qurre.API.Controllers.Camera;

namespace Qurre.API
{
    public static class Map
    {
        public static CassieList Cassies { get; internal set; } = new ();

        public static List<LightPoint> Lights { get; } = new ();
        public static List<Primitive> Primitives { get; } = new ();
        public static List<ShootingTarget> ShootingTargets { get; } = new ();
        public static List<WorkStation> WorkStations { get; } = new ();

        public static List<Camera> Cameras { get; } = new ();
        public static List<Door> Doors { get; } = new ();
        public static List<Generator> Generators { get; } = new ();
        public static List<Lift> Lifts { get; } = new ();
        public static List<Locker> Lockers { get; } = new ();
        public static List<Ragdoll> Ragdolls { get; } = new ();
        public static List<Room> Rooms { get; } = new ();
        public static List<Sinkhole> Sinkholes { get; } = new ();
        public static List<Tesla> Teslas { get; } = new ();
        public static List<Window> Windows { get; } = new ();

        public static List<Pickup> Pickups
        {
            get // not recommended, change later
            {
                List<Pickup> pickups = new ();

                foreach (ItemPickupBase itemPickupBase in Object.FindObjectsOfType<ItemPickupBase>())
                {
                    if (Pickup.Get(itemPickupBase) is Pickup pickup)
                    {
                        pickups.Add(pickup);
                    }
                }

                return pickups;
            }
        }

        public static AmbientSoundPlayer AmbientSoundPlayer { get; internal set; }


        public static MapBroadcast Broadcast(string message, ushort duration, bool instant = false) =>
            new (message, duration, instant, false);

        public static MapBroadcast BroadcastAdmin(string message, ushort duration, bool instant = false) =>
            new (message, duration, instant, true);
    }
}