using Qurre.API.Controllers;
using Qurre.API.Controllers.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API
{
    static public class Map
    {
        static public CassieList Cassies { get; internal set; } = new();

        static public List<LightPoint> Lights { get; } = new();
        static public List<Primitive> Primitives { get; } = new();
        static public List<ShootingTarget> ShootingTargets { get; } = new();
        static public List<WorkStation> WorkStations { get; } = new();

        public static List<Door> Doors { get; } = new();
        public static List<Generator> Generators { get; } = new();
        public static List<Locker> Lockers { get; } = new();
        public static List<Ragdoll> Ragdolls { get; } = new();
        public static List<Sinkhole> Sinkholes { get; } = new();
        static public List<Tesla> Teslas { get; } = new();
        public static List<Window> Windows { get; } = new();

        static public AmbientSoundPlayer AmbientSoundPlayer { get; internal set; }
    }
}