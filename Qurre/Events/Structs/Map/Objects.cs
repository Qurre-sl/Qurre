using JetBrains.Annotations;
using Qurre.API;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class TriggerTeslaEvent : IBaseEvent
{
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

    private const uint EventID = MapEvents.TriggerTesla;
}

[PublicAPI]
public class WorkStationUpdateEvent : IBaseEvent
{
    internal WorkStationUpdateEvent(WorkStation station)
    {
        Station = station;
        Allowed = true;
    }

    public WorkStation Station { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = MapEvents.WorkStationUpdate;
}