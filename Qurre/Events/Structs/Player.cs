using LiteNetLib;
using Qurre.API;
using Qurre.Internal.EventsManager;
namespace Qurre.Events.Structs
{
    [Register(PlayerEvents.Preauth)]
    public struct PreauthEvent : IBaseEvent
    {
        public string UserId { get; }
        public string Ip { get; }
        public CentralAuthPreauthFlags Flags { get; }
        public string Region { get; }
        public byte[] Signature { get; }
        public ConnectionRequest Request { get; }
    }

    [Register(PlayerEvents.Join)]
    public struct JoinEvent : IBaseEvent
    {
        public Player Player { get; }
    }

    [Register(PlayerEvents.Leave)]
    public struct LeaveEvent
    {
        public Player Player { get; }
    }

    [Register(PlayerEvents.CheckReserveSlot)]
    public struct CheckReserveSlotEvent : IBaseEvent
    {
        public string UserId { get; }
        public bool Allowed { get; set; }

        internal CheckReserveSlotEvent(string userid, bool allowed = true)
        {
            UserId = userid;
            Allowed = allowed;
        }
    }
}