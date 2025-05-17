using JetBrains.Annotations;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class CheaterReportEvent : IBaseEvent
{
    private const uint EventID = ServerEvents.CheaterReport;

    internal CheaterReportEvent(Player issuer, Player target, string reason)
    {
        Issuer = issuer;
        Target = target;
        Reason = reason;
        Allowed = true;
    }

    public Player Issuer { get; }
    public Player Target { get; }
    public string Reason { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class LocalReportEvent : IBaseEvent
{
    private const uint EventID = ServerEvents.LocalReport;

    internal LocalReportEvent(Player issuer, Player target, string reason)
    {
        Issuer = issuer;
        Target = target;
        Reason = reason;
        Allowed = true;
    }

    public Player Issuer { get; }
    public Player Target { get; }
    public string Reason { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}