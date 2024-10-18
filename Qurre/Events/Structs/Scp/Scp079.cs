using JetBrains.Annotations;
using PlayerRoles.PlayableScps.Scp079;
using Qurre.API;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class ActivateGeneratorEvent : IBaseEvent
{
    internal ActivateGeneratorEvent(Generator generator)
    {
        Generator = generator;
        Allowed = true;
    }

    public Generator Generator { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = ScpEvents.ActivateGenerator;
}

[PublicAPI]
public class Scp079GetExpEvent : IBaseEvent
{
    internal Scp079GetExpEvent(Player player, Scp079HudTranslation type, int amount)
    {
        Player = player;
        Type = type;
        Amount = amount;
        Allowed = true;
    }

    public Player Player { get; }
    public Scp079HudTranslation Type { get; }
    public int Amount { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = ScpEvents.Scp079GetExp;
}

[PublicAPI]
public class Scp079NewLvlEvent : IBaseEvent
{
    internal Scp079NewLvlEvent(Player player, int level)
    {
        Player = player;
        Level = level;
        Allowed = true;
    }

    public Player Player { get; }
    public int Level { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = ScpEvents.Scp079NewLvl;
}

[PublicAPI]
public class Scp079RecontainEvent : IBaseEvent
{
    internal Scp079RecontainEvent()
    {
    }

    public uint EventId { get; } = EventID;

    private const uint EventID = ScpEvents.Scp079Recontain;
}

[PublicAPI]
public class GeneratorStatusEvent : IBaseEvent
{
    internal GeneratorStatusEvent(int enragedCount, int totalCount)
    {
        EnragedCount = enragedCount;
        TotalCount = totalCount;
    }

    public int EnragedCount { get; }
    public int TotalCount { get; }
    public uint EventId { get; } = EventID;

    private const uint EventID = ScpEvents.GeneratorStatus;
}