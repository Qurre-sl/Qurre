using Qurre.Internal.EventsManager;
namespace Qurre.Events.Structs
{
    [Register(PlayerEvents.Preauth)]
    internal struct PreauthEvent
    {
        public string UserId { get; }
    }
}