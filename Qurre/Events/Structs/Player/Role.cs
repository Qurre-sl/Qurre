using JetBrains.Annotations;
using PlayerRoles;
using Qurre.API.Controllers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class SpawnEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Spawn;

    internal SpawnEvent(Player player, RoleTypeId role, Vector3 position, Vector3 rotation)
    {
        Player = player;
        Role = role;
        Position = position;
        Rotation = rotation;
    }

    public Player Player { get; }
    public RoleTypeId Role { get; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class ChangeRoleEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.ChangeRole;

    internal ChangeRoleEvent(Player player, PlayerRoleBase oldRole, RoleTypeId role, RoleChangeReason reason)
    {
        Player = player;
        OldRole = oldRole;
        Role = role;
        Reason = reason;
        Allowed = true;
    }

    public Player Player { get; }
    public PlayerRoleBase OldRole { get; }
    public RoleTypeId Role { get; set; }
    public RoleChangeReason Reason { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class EscapeEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Escape;

    internal EscapeEvent(Player player, RoleTypeId role)
    {
        Player = player;
        Role = role;
        Allowed = true;
    }

    public Player Player { get; }
    public RoleTypeId Role { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}