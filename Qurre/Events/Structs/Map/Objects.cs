using JetBrains.Annotations;
using PlayerRoles.Voice;
using Qurre.API.Entities.Characters;
using Qurre.API.Entities.Environment;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class TriggerTeslaEvent : ICancellableEvent
{
    internal TriggerTeslaEvent(Player player, ITesla tesla, bool inIdlingRange, bool inRageRange)
    {
        Player = player;
        Tesla = tesla;
        InIdlingRange = inIdlingRange;
        InRageRange = inRageRange;
    }

    /// <inheritdoc />
    public uint EventId { get; } = MapEvents.TriggerTesla;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public ITesla Tesla { get; }
    public bool InIdlingRange { get; }
    public bool InRageRange { get; set; }
}

[PublicAPI]
public class WorkStationUpdateEvent : ICancellableEvent
{
    internal WorkStationUpdateEvent(IWorkStation station)
    {
        Station = station;
    }

    /// <inheritdoc />
    public uint EventId { get; } = MapEvents.WorkStationUpdate;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public IWorkStation Station { get; }
}

[PublicAPI]
public class IntercomSetStateEvent : ICancellableEvent
{
    internal IntercomSetStateEvent(IntercomState state)
    {
        State = state;
    }

    /// <inheritdoc />
    public uint EventId { get; } = MapEvents.IntercomSetState;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public IntercomState State { get; set; }
}