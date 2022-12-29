using Qurre.API;

namespace Qurre.Events.Structs
{
    public class CheaterReportEvent : IBaseEvent
    {
        public uint EventId { get; } = ServerEvents.CheaterReport;

        public Player Issuer { get; }
        public Player Target { get; }
        public string Reason { get; }
        public bool Allowed { get; set; }

        internal CheaterReportEvent(Player issuer, Player target, string reason)
        {
            Issuer = issuer;
            Target = target;
            Reason = reason;
            Allowed = true;
        }
    }

    public class LocalReportEvent : IBaseEvent
    {
        public uint EventId { get; } = ServerEvents.LocalReport;

        public Player Issuer { get; }
        public Player Target { get; }
        public string Reason { get; }
        public bool Allowed { get; set; }

        internal LocalReportEvent(Player issuer, Player target, string reason)
        {
            Issuer = issuer;
            Target = target;
            Reason = reason;
            Allowed = true;
        }
    }
}