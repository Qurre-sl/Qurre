using JetBrains.Annotations;
using Qurre.API.Entities.AdminToys;
using Qurre.API.Entities.Characters;
using Qurre.API.Entities.Doors;
using Qurre.API.Entities.Environment;
using Qurre.API.Entities.Hazards;
using Qurre.API.Entities.Structures;

namespace Qurre.API.Entities;

[PublicAPI]
public static class EntityFactories
{
    public static AdminToyFactory AdminToy { get; } = new();
    public static CharacterFactory Character { get; } = new();
    public static DoorFactory Door { get; } = new();
    public static EnvironmentFactory Environment { get; } = new();
    public static HazardFactory Hazard { get; } = new();
    public static StructureFactory Structure { get; } = new();
}