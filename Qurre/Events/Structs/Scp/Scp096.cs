using JetBrains.Annotations;
using PlayerRoles.PlayableScps.Scp096;
using Qurre.API;
using Qurre.API.Entities.Characters;
using Qurre.API.Enums;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp096SetStateEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp096SetState;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pl"></param>
    /// <param name="state"></param>
    internal Scp096SetStateEvent(Player pl, Scp096State state)
    {
        Player = pl;
        State = state;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pl"></param>
    /// <param name="state"></param>
    internal Scp096SetStateEvent(Player pl, Scp096RageState state)
    {
        Player = pl;

        State = state switch
        {
            Scp096RageState.Calming => Scp096State.Calming,
            Scp096RageState.Enraged => Scp096State.Enraged,
            Scp096RageState.Distressed => Scp096State.Distressed,
            Scp096RageState.Docile => Scp096State.Docile,
            _ => Scp096State.Unknown
        };
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
    public Scp096State State { get; }
    
    #endregion
}

[PublicAPI]
public class Scp096AddTargetEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp096AddTarget;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scp"></param>
    /// <param name="target"></param>
    /// <param name="isLooking"></param>
    internal Scp096AddTargetEvent(ReferenceHub scp, ReferenceHub target, bool isLooking)
    {
        Scp = scp.GetPlayer() ?? Server.Host;
        Target = target.GetPlayer() ?? Server.Host;
        IsLooking = isLooking;
    }
    
    #endregion

    #region Public API

    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    /// <summary></summary>
    public Player Scp { get; }
    
    /// <summary></summary>
    public Player Target { get; }
    
    /// <summary></summary>
    public bool IsLooking { get; }
    
    #endregion
}