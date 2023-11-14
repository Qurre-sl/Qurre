using InventorySystem.Items.ThrowableProjectiles;
using Qurre.API;
using UnityEngine;

namespace Qurre.Events.Structs
{
    public class PlayerFlashedEvent : IBaseEvent
    {
        public uint EventId { get; } = EffectEvents.Flashed;

        public Player Player { get; }
        public Player Thrower { get; }
        public FlashbangGrenade Grenade { get; }
        public Vector3 Position { get; }
        public bool Allowed { get; set; }

        internal PlayerFlashedEvent(Player player, FlashbangGrenade grenade, float duration)
        {
            Player = player;
            Grenade = grenade;

            Thrower = grenade.PreviousOwner.Hub.GetPlayer();
            Position = grenade.transform.position;

            Allowed = duration > grenade._minimalEffectDuration;
        }
    }
}