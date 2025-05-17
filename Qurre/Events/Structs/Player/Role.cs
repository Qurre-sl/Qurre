using JetBrains.Annotations;
using PlayerRoles;
using Qurre.API.Entities.Characters;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class SpawnEvent : IBaseEvent
{
    internal SpawnEvent(Player player, RoleTypeId role, Vector3 position, Vector3 rotation)
    {
        Player = player;
        Role = role;
        Position = position;
        Rotation = rotation;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Spawn;

    public Player Player { get; }
    public RoleTypeId Role { get; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
}

[PublicAPI]
public class ChangeRoleEvent : ICancellableEvent
{
    internal ChangeRoleEvent(Player player, PlayerRoleBase oldRole, RoleTypeId role, RoleChangeReason reason)
    {
        Player = player;
        OldRole = oldRole;
        Role = role;
        Reason = reason;
        IsAllowed = true;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.ChangeRole;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; }

    public Player Player { get; }
    public PlayerRoleBase OldRole { get; }
    public RoleTypeId Role { get; set; }
    public RoleChangeReason Reason { get; set; }
}

[PublicAPI]
public class EscapeEvent : ICancellableEvent
{
    internal EscapeEvent(Player player, RoleTypeId role)
    {
        Player = player;
        Role = role;
        IsAllowed = true;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Escape;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; }

    public Player Player { get; }
    public RoleTypeId Role { get; set; }
}