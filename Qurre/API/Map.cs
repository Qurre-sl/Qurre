namespace Qurre.API;

using InventorySystem.Items.Pickups;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Structs;
using System.Collections.Generic;

static public class Map
{
    static public CassieList Cassies { get; internal set; } = new();

    static public List<LightPoint> Lights { get; } = new();
    static public List<Primitive> Primitives { get; } = new();
    static public List<ShootingTarget> ShootingTargets { get; } = new();
    static public List<WorkStation> WorkStations { get; } = new();

    static public List<Camera> Cameras { get; } = new();
    static public List<Door> Doors { get; } = new();
    static public List<Generator> Generators { get; } = new();
    static public List<Lift> Lifts { get; } = new();
    static public List<Locker> Lockers { get; } = new();
    static public List<Ragdoll> Ragdolls { get; } = new();
    static public List<Room> Rooms { get; } = new();
    static public List<Sinkhole> Sinkholes { get; } = new();
    static public List<Tesla> Teslas { get; } = new();
    static public List<Window> Windows { get; } = new();
    static public List<Pickup> Pickups
    {
        get // not recommended, change later
        {
            List<Pickup> pickups = new();
            foreach (ItemPickupBase itemPickupBase in UnityEngine.Object.FindObjectsOfType<ItemPickupBase>())
            {
                if (Pickup.Get(itemPickupBase) is Pickup pickup)
                    pickups.Add(pickup);
            }
            return pickups;
        }
    }

    static public AmbientSoundPlayer AmbientSoundPlayer { get; internal set; }


    static public MapBroadcast Broadcast(string message, ushort duration, bool instant = false) =>
        new(message, duration, instant, false);
    static public MapBroadcast BroadcastAdmin(string message, ushort duration, bool instant = false) =>
        new(message, duration, instant, true);
}