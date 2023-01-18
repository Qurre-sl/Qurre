using PlayerStatsSystem;

namespace Qurre.API.Classification.Player
{
    public class HealthInfomation
    {
        private readonly API.Player _player;

        internal HealthInfomation(API.Player pl) => _player = pl;

        public PlayerStats PlayerStats => _player.ReferenceHub.playerStats;

        public float Hp
        {
            get => ((HealthStat)PlayerStats.StatModules[0]).CurValue;
            set => ((HealthStat)PlayerStats.StatModules[0]).CurValue = value;
        }

        public float MaxHp
        {
            get => ((AhpStat)PlayerStats.StatModules[1]).MaxValue;
            set => ((AhpStat)PlayerStats.StatModules[1])._maxSoFar = value;
        }

        public float Ahp
        {
            get => ((AhpStat)PlayerStats.StatModules[0]).CurValue;
            set => ((AhpStat)PlayerStats.StatModules[0]).CurValue = value;
        }

        public float MaxAhp
        {
            get => ((AhpStat)PlayerStats.StatModules[1]).MaxValue;
            set => ((AhpStat)PlayerStats.StatModules[1])._maxSoFar = value;
        }

        public void Kill(DeathTranslation deathReason) => PlayerStats.KillPlayer(new UniversalDamageHandler(-1, deathReason));

        public void Kill(string deathReason = "") => PlayerStats.KillPlayer(new CustomReasonDamageHandler(deathReason));

        public bool DealDamage(DamageHandlerBase handler)
            => PlayerStats.DealDamage(handler);

        public bool Damage(float damage, string deathReason)
            => DealDamage(new CustomReasonDamageHandler(deathReason, damage));

        public bool Damage(float damage, DeathTranslation deathReason)
            => DealDamage(new UniversalDamageHandler(damage, deathReason));

        public bool Damage(float damage, DeathTranslation deathReason, API.Player attacker)
        {
            if (attacker is null)
            {
                return DealDamage(new UniversalDamageHandler(damage, deathReason));
            }

            return DealDamage(new ScpDamageHandler(attacker.ReferenceHub, damage, deathReason));
        }
    }
}