using JetBrains.Annotations;
using PlayerRoles.Voice;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class TriggerTeslaEvent : IBaseEvent
{
    private const uint EventID = MapEvents.TriggerTesla;

    internal TriggerTeslaEvent(Player player, Tesla tesla, bool inIdlingRange, bool inRageRange)
    {
        Player = player;
        Tesla = tesla;
        InIdlingRange = inIdlingRange;
        InRageRange = inRageRange;
        Allowed = true;
    }

    public Player Player { get; }
    public Tesla Tesla { get; }
    public bool InIdlingRange { get; }
    public bool InRageRange { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class WorkStationUpdateEvent : IBaseEvent
{
    private const uint EventID = MapEvents.WorkStationUpdate;

    internal WorkStationUpdateEvent(WorkStation station)
    {
        Station = station;
        Allowed = true;
    }

    public WorkStation Station { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class IntercomSetStateEvent : IBaseEvent
{
    private const uint EventID = MapEvents.IntercomSetState;

    internal IntercomSetStateEvent(IntercomState state)
    {
        State = state;
        Allowed = true;
    }

    public IntercomState State { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}