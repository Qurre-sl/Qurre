using PlayerRoles;
using Qurre.API;
using UnityEngine;

namespace Qurre.Events.Structs
{
    public class SpawnEvent : IBaseEvent
    {
        internal SpawnEvent(Player player, RoleTypeId role, Vector3 position, Vector3 rotation)
        {
            Player = player;
            Role = role;
            Position = position;
            Rotation = rotation;
        }

        public uint EventId { get; } = PlayerEvents.Spawn;

        public Player Player { get; }
        public RoleTypeId Role { get; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
    }

    public class ChangeRoleEvent : IBaseEvent
    {
        internal ChangeRoleEvent(Player player, PlayerRoleBase oldRole, RoleTypeId role, RoleChangeReason reason)
        {
            Player = player;
            OldRole = oldRole;
            Role = role;
            Reason = reason;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.ChangeRole;

        public Player Player { get; }
        public PlayerRoleBase OldRole { get; }
        public RoleTypeId Role { get; set; }
        public RoleChangeReason Reason { get; set; }
        public bool Allowed { get; set; }
    }

    public class EscapeEvent : IBaseEvent
    {
        internal EscapeEvent(Player player, RoleTypeId role)
        {
            Player = player;
            Role = role;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.Escape;

        public Player Player { get; }
        public RoleTypeId Role { get; set; }
        public bool Allowed { get; set; }
    }
}