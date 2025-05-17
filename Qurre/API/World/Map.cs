using System.Collections.Generic;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Structs;
using Camera = Qurre.API.Controllers.Camera;

namespace Qurre.API.World;

[PublicAPI]
public static class Map
{
    public static CassieList Cassies { get; internal set; } = new();

    public static List<LightPoint> Lights { get; } = [];
    public static List<Primitive> Primitives { get; } = [];
    public static List<ShootingTarget> ShootingTargets { get; } = [];
    public static List<Speaker> Speakers { get; } = [];
    public static List<WorkStation> WorkStations { get; } = [];

    public static List<Camera> Cameras { get; } = [];
    public static List<Door> Doors { get; } = [];
    public static List<Generator> Generators { get; } = [];
    public static List<Lift> Lifts { get; } = [];
    public static List<Locker> Lockers { get; } = [];
    public static List<Corpse> Corpses { get; } = [];
    public static List<Room> Rooms { get; } = [];
    public static List<Sinkhole> Sinkholes { get; } = [];
    public static List<Tesla> Teslas { get; } = [];
    public static List<Window> Windows { get; } = [];

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