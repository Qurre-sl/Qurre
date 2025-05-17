using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PlayerRoles;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons;

[PublicAPI]
public readonly struct CachedPlayer : IEquatable<CachedPlayer>
{
    public Player Player { get; }
    public Vector3 Position { get; }
    public RoleTypeId Role { get; }
    public string Tag { get; }
    public IReadOnlyCollection<ItemType> Inventory { get; }
    public string InventoryHash { get; }

    internal CachedPlayer(Player player)
    {
        Player = player;
        Position = player.MovementState.Position;
        Role = !player.RoleInformation.IsAlive ? player.RoleInformation.CachedRole : player.RoleInformation.Role;
        Tag = player.Tag;
        Inventory = [.. player.Inventory.Base.UserInventory.Items.Select(x => x.Value.ItemTypeId)];
        InventoryHash = string.Join(',', Inventory.Select(x => $"{x}"));
    }

    public override bool Equals(object? obj)
    {
        return obj is CachedPlayer other && Equals(other);
    }

    public bool Equals(CachedPlayer obj)
    {
        return this == obj;
    }

    public override int GetHashCode()
    {
        return Tuple.Create(Player, Position, Role, Tag, InventoryHash).GetHashCode();
    }

    public override string ToString()
    {
        return $"{Player.UserInformation.Nickname} - {Player.UserInformation.UserId} - {Role}";
    }

    public static bool operator ==(CachedPlayer a, CachedPlayer b)
    {
        return a.Player == b.Player && a.Position == b.Position && a.Role == b.Role && a.Tag == b.Tag &&
               a.InventoryHash == b.InventoryHash;
    }

    public static bool operator !=(CachedPlayer a, CachedPlayer b)
    {
        return !(a == b);
    }
}