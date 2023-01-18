using Qurre.API;

namespace Qurre.Events.Structs
{
    public class CheaterReportEvent : IBaseEvent
    {
        internal CheaterReportEvent(Player issuer, Player target, string reason)
        {
            Issuer = issuer;
            Target = target;
            Reason = reason;
            Allowed = true;
        }

        public uint EventId { get; } = ServerEvents.CheaterReport;

        public Player Issuer { get; }
        public Player Target { get; }
        public string Reason { get; }
        public bool Allowed { get; set; }
    }

    public class LocalReportEvent : IBaseEvent
    {
        internal LocalReportEvent(Player issuer, Player target, string reason)
        {
            Issuer = issuer;
            Target = target;
            Reason = reason;
            Allowed = true;
        }

        public uint EventId { get; } = ServerEvents.LocalReport;

        public Player Issuer { get; }
        public Player Target { get; }
        public string Reason { get; }
        public bool Allowed { get; set; }
    }
}