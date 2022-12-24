using LiteNetLib;
using Qurre.API;
using Qurre.Internal.EventsManager;
namespace Qurre.Events.Structs
{
    [Register(PlayerEvents.Preauth)]
    public struct PreauthEvent
    {
        public string UserId { get; }
        public string Ip { get; }
        public CentralAuthPreauthFlags Flags { get; }
        public string Region { get; }
        public byte[] Signature { get; }
        public ConnectionRequest Request { get; }
    }

    [Register(PlayerEvents.Join)]
    public struct JoinEvent
    {
        public Player Player { get; }
    }

    [Register(PlayerEvents.Leave)]
    public struct LeaveEvent
    {
        public Player Player { get; }
    }

    [Register(PlayerEvents.CheckReserveSlot)]
    public struct CheckReserveSlotEvent
    {
        public Player Player { get; }
    }
}