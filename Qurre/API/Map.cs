using System.Collections.Generic;
using InventorySystem.Items.Pickups;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Structs;
using UnityEngine;
using Camera = Qurre.API.Controllers.Camera;

namespace Qurre.API;

[PublicAPI]
public static class Map
{
    public static CassieList Cassies { get; internal set; } = new();

    public static List<LightPoint> Lights { get; } = [];
    public static List<Primitive> Primitives { get; } = [];
    public static List<ShootingTarget> ShootingTargets { get; } = [];
    public static List<WorkStation> WorkStations { get; } = [];

    public static List<Camera> Cameras { get; } = [];
    public static List<Door> Doors { get; } = [];
    public static List<Generator> Generators { get; } = [];
    public static List<Lift> Lifts { get; } = [];
    public static List<Locker> Lockers { get; } = [];
    public static List<Ragdoll> Ragdolls { get; } = [];
    public static List<Room> Rooms { get; } = [];
    public static List<Sinkhole> Sinkholes { get; } = [];
    public static List<Tesla> Teslas { get; } = [];
    public static List<Window> Windows { get; } = [];

    public static List<Pickup> Pickups
    {
        get // not recommended, change later
        {
            List<Pickup> pickups = [];
            foreach (ItemPickupBase itemPickupBase in Object.FindObjectsOfType<ItemPickupBase>())
                if (Pickup.Get(itemPickupBase) is { } pickup)
                    pickups.Add(pickup);
            return pickups;
        }
    }

    public static AmbientSoundPlayer? AmbientSoundPlayer { get; internal set; }


    public static MapBroadcast Broadcast(string message, ushort duration, bool instant = false)
    {
        return new MapBroadcast(message, duration, instant, false);
    }

    public static MapBroadcast BroadcastAdmin(string message, ushort duration, bool instant = false)
    {
        return new MapBroadcast(message, duration, instant, true);
    }
}