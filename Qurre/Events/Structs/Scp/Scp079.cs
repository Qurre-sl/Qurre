using JetBrains.Annotations;
using PlayerRoles.PlayableScps.Scp079;
using Qurre.API.Entities.Characters;
using Qurre.API.Entities.Structures;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class ActivateGeneratorEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.ActivateGenerator;

    #endregion

    #region Constructors

    internal ActivateGeneratorEvent(IGenerator generator)
    {
        Generator = generator;
    }
    
    #endregion

    /// <inheritdoc />
    public uint EventId { get; } = ScpEvents.ActivateGenerator;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    /// <summary></summary>
    public IGenerator Generator { get; }
}

[PublicAPI]
public class Scp079GetExpEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp079GetExp;
    
    #endregion

    #region Constructors

    internal Scp079GetExpEvent(Player player, Scp079HudTranslation type, int amount)
    {
        Player = player;
        Type = type;
        Amount = amount;
    }
    
    #endregion
    
    #region Public API

    /// <inheritdoc />
    public uint EventId { get; } = EventID;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    /// <summary></summary>
    public Player Player { get; }
    
    /// <summary></summary>
    public Scp079HudTranslation Type { get; }
    
    /// <summary></summary>
    public int Amount { get; set; }
    
    #endregion
}

[PublicAPI]
public class Scp079NewLvlEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp079NewLvl;

    #endregion

    #region Constructors

    internal Scp079NewLvlEvent(Player player, int level)
    {
        Player = player;
        Level = level;
    }
    
    #endregion
    
    #region Public API

    /// <inheritdoc />
    public uint EventId { get; } = EventID;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;
    
    /// <summary></summary>
    public Player Player { get; }
    
    /// <summary></summary>
    public int Level { get; set; }
    
    #endregion
}

[PublicAPI]
public class Scp079RecontainEvent : IBaseEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp079Recontain;

    #endregion

    #region Constructors

    internal Scp079RecontainEvent()
    {
    }
    
    #endregion
    
    #region Public API

    /// <inheritdoc />
    public uint EventId { get; } = EventID;
    
    #endregion
}

[PublicAPI]
public class GeneratorStatusEvent : IBaseEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.GeneratorStatus;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enragedCount"></param>
    /// <param name="totalCount"></param>
    internal GeneratorStatusEvent(int enragedCount, int totalCount)
    {
        EnragedCount = enragedCount;
        TotalCount = totalCount;
    }
    
    #endregion

    #region Public API
    
    /// <inheritdoc />
    public uint EventId { get; } = EventID;
    
    /// <summary></summary>
    public int EnragedCount { get; }
    
    /// <summary></summary>
    public int TotalCount { get; }
    
    #endregion
}