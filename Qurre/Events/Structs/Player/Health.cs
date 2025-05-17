using JetBrains.Annotations;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Entities.Characters;
using Qurre.API.Enums;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class DeadEvent : IBaseEvent
{
    internal DeadEvent(Player attacker, Player target, DamageHandlerBase damageInfo, DamageTypes type)
    {
        Attacker = attacker;
        Target = target;
        DamageType = type;
        DamageInfo = damageInfo;
        LiteType = damageInfo.GetLiteDamageTypes();
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Dead;

    public Player Attacker { get; }
    public Player Target { get; }
    public DamageTypes DamageType { get; }
    public DamagePrimitiveTypes LiteType { get; }
    public DamageHandlerBase DamageInfo { get; }
}

[PublicAPI]
public class DiesEvent : ICancellableEvent
{
    private DamagePrimitiveTypes _damagePrimitiveType = DamagePrimitiveTypes.Unknown;
    private DamageTypes _damageType = DamageTypes.Unknown;

    internal DiesEvent(Player attacker, Player target, DamageHandlerBase damageInfo)
    {
        Attacker = attacker;
        Target = target;
        DamageInfo = damageInfo;
    }

    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Dies;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Attacker { get; }
    public Player Target { get; }
    public DamageHandlerBase DamageInfo { get; }

    public DamageTypes DamageType
    {
        get
        {
            if (_damageType is DamageTypes.Unknown) _damageType = DamageInfo.GetDamageType();
            return _damageType;
        }
    }

    public DamagePrimitiveTypes DamagePrimitiveType
    {
        get
        {
            if (_damagePrimitiveType is DamagePrimitiveTypes.Unknown) _damagePrimitiveType = DamageInfo.GetLiteDamageTypes();
            return _damagePrimitiveType;
        }
    }
}

[PublicAPI]
public class DamageEvent : ICancellableEvent
{
    private DamagePrimitiveTypes _damagePrimitiveType = DamagePrimitiveTypes.Unknown;
    private DamageTypes _damageType = DamageTypes.Unknown;

    internal DamageEvent(Player attacker, Player target, DamageHandlerBase damageInfo, float damage)
    {
        Attacker = attacker;
        Target = target;
        DamageInfo = damageInfo;
        Damage = damage;
    }

    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Damage;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Attacker { get; }
    public Player Target { get; }
    public DamageHandlerBase DamageInfo { get; }
    public float Damage { get; set; }
    
    public DamageTypes DamageType
    {
        get
        {
            if (_damageType is DamageTypes.Unknown) _damageType = DamageInfo.GetDamageType();
            return _damageType;
        }
    }

    public DamagePrimitiveTypes DamagePrimitiveType
    {
        get
        {
            if (_damagePrimitiveType is DamagePrimitiveTypes.Unknown) _damagePrimitiveType = DamageInfo.GetLiteDamageTypes();
            return _damagePrimitiveType;
        }
    }
}

[PublicAPI]
public class AttackEvent : ICancellableEvent
{
    private DamagePrimitiveTypes _damagePrimitiveType = DamagePrimitiveTypes.Unknown;
    private DamageTypes _damagesType = DamageTypes.Unknown;

    internal AttackEvent(Player attacker, Player target, AttackerDamageHandler damageInfo, float damage,
        bool friendlyFire, bool isAllowed)
    {
        Attacker = attacker;
        Target = target;
        DamageInfo = damageInfo;
        Damage = damage;
        FriendlyFire = friendlyFire;
        IsAllowed = isAllowed;
    }

    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Attack;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; }

    public Player Attacker { get; }
    public Player Target { get; }
    public AttackerDamageHandler DamageInfo { get; }
    public float Damage { get; set; }
    public bool FriendlyFire { get; set; }

    public DamageTypes DamageType
    {
        get
        {
            if (_damagesType is DamageTypes.Unknown) _damagesType = DamageInfo.GetDamageType();
            return _damagesType;
        }
    }

    public DamagePrimitiveTypes DamagePrimitiveType
    {
        get
        {
            if (_damagePrimitiveType is DamagePrimitiveTypes.Unknown) _damagePrimitiveType = DamageInfo.GetLiteDamageTypes();
            return _damagePrimitiveType;
        }
    }
}

[PublicAPI]
public class HealEvent : ICancellableEvent
{
    internal HealEvent(Player player, float amount)
    {
        Player = player;
        Amount = amount;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Heal;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public float Amount { get; set; }
}