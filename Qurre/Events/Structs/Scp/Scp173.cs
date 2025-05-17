using JetBrains.Annotations;
using Qurre.API.Entities.Characters;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp173AddObserverEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp173AddObserver;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="scp"></param>
    internal Scp173AddObserverEvent(Player player, Player scp)
    {
        Player = player;
        Scp = scp;
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
    public Player Scp { get; }
    
    #endregion
}

[PublicAPI]
public class Scp173RemovedObserverEvent : IBaseEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp173RemovedObserver;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pl"></param>
    /// <param name="scp"></param>
    internal Scp173RemovedObserverEvent(Player pl, Player scp)
    {
        Player = pl;
        Scp = scp;
    }
    
    #endregion

    #region Public API

    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    /// <summary></summary>
    public Player Player { get; }
    
    /// <summary></summary>
    public Player Scp { get; }
    
    #endregion
}

[PublicAPI]
public class Scp173EnableSpeedEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp173EnableSpeed;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="value"></param>
    internal Scp173EnableSpeedEvent(Player player, bool value)
    {
        Player = player;
        Value = value;
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
    public bool Value { get; }
    
    #endregion
}