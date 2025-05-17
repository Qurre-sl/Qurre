using JetBrains.Annotations;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Objects;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class DeadEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Dead;

    internal DeadEvent(Player attacker, Player target, DamageHandlerBase damageInfo, DamageTypes type)
    {
        Attacker = attacker;
        Target = target;
        DamageType = type;
        DamageInfo = damageInfo;
        LiteType = damageInfo.GetLiteDamageTypes();
    }

    public Player Attacker { get; }
    public Player Target { get; }
    public DamageTypes DamageType { get; }
    public LiteDamageTypes LiteType { get; }
    public DamageHandlerBase DamageInfo { get; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class DiesEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Dies;
    private LiteDamageTypes _liteType = LiteDamageTypes.Unknown;

    private DamageTypes _type = DamageTypes.Unknown;

    internal DiesEvent(Player attacker, Player target, DamageHandlerBase damageInfo)
    {
        Attacker = attacker;
        Target = target;
        DamageInfo = damageInfo;
        Allowed = true;
    }

    public Player Attacker { get; }
    public Player Target { get; }
    public DamageHandlerBase DamageInfo { get; }
    public bool Allowed { get; set; }

    public DamageTypes DamageType
    {
        get
        {
            if (_type is DamageTypes.Unknown) _type = DamageInfo.GetDamageType();
            return _type;
        }
    }

    public LiteDamageTypes LiteType
    {
        get
        {
            if (_liteType is LiteDamageTypes.Unknown) _liteType = DamageInfo.GetLiteDamageTypes();
            return _liteType;
        }
    }

    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class DamageEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Damage;
    private LiteDamageTypes _liteType = LiteDamageTypes.Unknown;

    private DamageTypes _type = DamageTypes.Unknown;

    internal DamageEvent(Player attacker, Player target, DamageHandlerBase damageInfo, float damage)
    {
        Attacker = attacker;
        Target = target;
        DamageInfo = damageInfo;
        Damage = damage;
        Allowed = true;
    }

    public Player Attacker { get; }
    public Player Target { get; }
    public DamageHandlerBase DamageInfo { get; }
    public float Damage { get; set; }
    public bool Allowed { get; set; }

    public DamageTypes DamageType
    {
        get
        {
            if (_type is DamageTypes.Unknown) _type = DamageInfo.GetDamageType();
            return _type;
        }
    }

    public LiteDamageTypes LiteType
    {
        get
        {
            if (_liteType is LiteDamageTypes.Unknown) _liteType = DamageInfo.GetLiteDamageTypes();
            return _liteType;
        }
    }

    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class AttackEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Attack;
    private LiteDamageTypes _liteType = LiteDamageTypes.Unknown;

    private DamageTypes _type = DamageTypes.Unknown;

    internal AttackEvent(Player attacker, Player target, AttackerDamageHandler damageInfo, float damage,
        bool friendlyFire, bool allowed)
    {
        Attacker = attacker;
        Target = target;
        DamageInfo = damageInfo;
        Damage = damage;
        FriendlyFire = friendlyFire;
        Allowed = allowed;
    }

    public Player Attacker { get; }
    public Player Target { get; }
    public AttackerDamageHandler DamageInfo { get; }
    public float Damage { get; set; }
    public bool FriendlyFire { get; set; }
    public bool Allowed { get; set; }

    public DamageTypes DamageType
    {
        get
        {
            if (_type is DamageTypes.Unknown) _type = DamageInfo.GetDamageType();
            return _type;
        }
    }

    public LiteDamageTypes LiteType
    {
        get
        {
            if (_liteType is LiteDamageTypes.Unknown) _liteType = DamageInfo.GetLiteDamageTypes();
            return _liteType;
        }
    }

    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class HealEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Heal;

    internal HealEvent(Player player, float amount)
    {
        Player = player;
        Amount = amount;
        Allowed = true;
    }

    public Player Player { get; }
    public float Amount { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}