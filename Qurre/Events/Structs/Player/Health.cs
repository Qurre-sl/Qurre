using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Objects;

namespace Qurre.Events.Structs
{
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

        public uint EventId { get; } = PlayerEvents.Dead;

        public Player Attacker { get; }
        public Player Target { get; }
        public DamageTypes DamageType { get; }
        public LiteDamageTypes LiteType { get; }
        public DamageHandlerBase DamageInfo { get; }
    }

    public class DiesEvent : IBaseEvent
    {
        private DamageTypes _type = DamageTypes.Unknow;
        private LiteDamageTypes _liteType = LiteDamageTypes.Unknow;

        internal DiesEvent(Player attacker, Player target, DamageHandlerBase damageInfo)
        {
            Attacker = attacker;
            Target = target;
            DamageInfo = damageInfo;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.Dies;

        public Player Attacker { get; }
        public Player Target { get; }
        public DamageHandlerBase DamageInfo { get; }
        public bool Allowed { get; set; }

        public DamageTypes DamageType
        {
            get
            {
                if (_type is DamageTypes.Unknow)
                {
                    _type = DamageInfo.GetDamageType();
                }

                return _type;
            }
        }

        public LiteDamageTypes LiteType
        {
            get
            {
                if (_liteType is LiteDamageTypes.Unknow)
                {
                    _liteType = DamageInfo.GetLiteDamageTypes();
                }

                return _liteType;
            }
        }
    }

    public class DamageEvent : IBaseEvent
    {
        private DamageTypes _type = DamageTypes.Unknow;
        private LiteDamageTypes _liteType = LiteDamageTypes.Unknow;

        internal DamageEvent(Player attacker, Player target, DamageHandlerBase damageInfo, float damage)
        {
            Attacker = attacker;
            Target = target;
            DamageInfo = damageInfo;
            Damage = damage;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.Damage;

        public Player Attacker { get; }
        public Player Target { get; }
        public DamageHandlerBase DamageInfo { get; }
        public float Damage { get; set; }
        public bool Allowed { get; set; }

        public DamageTypes DamageType
        {
            get
            {
                if (_type is DamageTypes.Unknow)
                {
                    _type = DamageInfo.GetDamageType();
                }

                return _type;
            }
        }

        public LiteDamageTypes LiteType
        {
            get
            {
                if (_liteType is LiteDamageTypes.Unknow)
                {
                    _liteType = DamageInfo.GetLiteDamageTypes();
                }

                return _liteType;
            }
        }
    }

    public class AttackEvent : IBaseEvent
    {
        private DamageTypes _type = DamageTypes.Unknow;
        private LiteDamageTypes _liteType = LiteDamageTypes.Unknow;

        internal AttackEvent(Player attacker, Player target, AttackerDamageHandler damageInfo, float damage, bool friendlyFire, bool allowed)
        {
            Attacker = attacker;
            Target = target;
            DamageInfo = damageInfo;
            Damage = damage;
            FriendlyFire = friendlyFire;
            Allowed = allowed;
        }

        public uint EventId { get; } = PlayerEvents.Attack;

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
                if (_type is DamageTypes.Unknow)
                {
                    _type = DamageInfo.GetDamageType();
                }

                return _type;
            }
        }

        public LiteDamageTypes LiteType
        {
            get
            {
                if (_liteType is LiteDamageTypes.Unknow)
                {
                    _liteType = DamageInfo.GetLiteDamageTypes();
                }

                return _liteType;
            }
        }
    }
}