using JetBrains.Annotations;
using Qurre.API;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class CheaterReportEvent : IBaseEvent
{
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
    public uint EventId { get; } = ServerEvents.CheaterReport;
}

[PublicAPI]
public class LocalReportEvent : IBaseEvent
{
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
    public uint EventId { get; } = ServerEvents.LocalReport;
}