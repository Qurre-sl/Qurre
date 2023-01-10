using PlayerRoles;
using Qurre.API;
using UnityEngine;

namespace Qurre.Events.Structs
{
    public class SpawnEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.Spawn;

        public Player Player { get; }
        public RoleTypeId Role { get; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        internal SpawnEvent(Player player, RoleTypeId role, Vector3 position, Vector3 rotation)
        {
            Player = player;
            Role = role;
            Position = position;
            Rotation = rotation;
        }
    }
}