using InventorySystem.Items.ThrowableProjectiles;
using JetBrains.Annotations;
using Qurre.API;
using Qurre.API.Entities.Characters;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class PlayerFlashedEvent : ICancellableEvent
{
    internal PlayerFlashedEvent(Player player, FlashbangGrenade grenade, float duration)
    {
        Player = player;
        Grenade = grenade;

        Thrower = grenade.PreviousOwner.Hub.GetPlayer() ?? Server.Host;
        Position = grenade.transform.position;

        IsAllowed = duration > grenade._minimalEffectDuration;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = EffectEvents.Flashed;

    /// <inheritdoc />
    public bool IsAllowed { get; set; }
    
    public Player Player { get; }
    public Player Thrower { get; }
    public FlashbangGrenade Grenade { get; }
    public Vector3 Position { get; }
}