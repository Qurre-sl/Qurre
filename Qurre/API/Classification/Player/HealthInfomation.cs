using PlayerStatsSystem;
using System.Collections.Generic;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    using System.Linq;
    using UnityEngine;

    public sealed class HealthInfomation
    {
        internal float _maxHp = -1;
        private readonly Player _player;
        internal HealthInfomation(Player pl) => _player = pl;

        public PlayerStats PlayerStats => _player.ReferenceHub.playerStats;

        public float Hp
        {
            get => ((HealthStat)PlayerStats.StatModules[0]).CurValue;
            set => ((HealthStat)PlayerStats.StatModules[0]).CurValue = value;
        }
        public float MaxHp
        {
            get => ((HealthStat)PlayerStats.StatModules[0]).MaxValue;
            set => _maxHp = value;
        }

        public List<AhpStat.AhpProcess> AhpActiveProcesses => ((AhpStat)PlayerStats.StatModules[1])._activeProcesses;
        public float Ahp
        {
            get => ((AhpStat)PlayerStats.StatModules[1]).CurValue;
            set
            {
                if (value > MaxAhp)
                    MaxAhp = Mathf.CeilToInt(value);

                AhpStat.AhpProcess process = AhpActiveProcesses.FirstOrDefault();

                if (process is null)
                    AddAhp(value, MaxAhp);
                else
                {
                    process.Limit = MaxAhp;
                    process.CurrentAmount = value;
                }
            }
        }
        public float MaxAhp
        {
            get => ((AhpStat)PlayerStats.StatModules[1]).MaxValue;
            set => ((AhpStat)PlayerStats.StatModules[1])._maxSoFar = value;
        }

        public float Stamina
        {
            get => ((StaminaStat)PlayerStats.StatModules[2]).CurValue * 100;
            set => ((StaminaStat)PlayerStats.StatModules[2]).CurValue = Mathf.Clamp01(value / 100);
        }

        public void Heal(float amount, bool instant)
        {
            if (instant)
                Hp += amount;
            else
                ((HealthStat)PlayerStats.StatModules[0]).ServerHeal(amount);
        }

        public void AddAhp(float amount, float limit, float decay = 0, float efficacy = 0.7f, float sustain = 0, bool persistant = false)
            => ((AhpStat)PlayerStats.StatModules[1]).ServerAddProcess(amount, limit, decay, efficacy, sustain, persistant);

        public void AddStamina(float value)
            => ((StaminaStat)PlayerStats.StatModules[2]).ModifyAmount(value / 100);

        public void Kill(DeathTranslation deathReason) => PlayerStats.KillPlayer(new UniversalDamageHandler(-1, deathReason));
        public void Kill(string deathReason = "") => PlayerStats.KillPlayer(new CustomReasonDamageHandler(deathReason));

        public bool DealDamage(DamageHandlerBase handler)
            => PlayerStats.DealDamage(handler);
        public bool Damage(float damage, string deathReason)
            => DealDamage(new CustomReasonDamageHandler(deathReason, damage));
        public bool Damage(float damage, DeathTranslation deathReason)
            => DealDamage(new UniversalDamageHandler(damage, deathReason));
        public bool Damage(float damage, DeathTranslation deathReason, Player attacker)
        {
            if (attacker is null) return DealDamage(new UniversalDamageHandler(damage, deathReason));
            return DealDamage(new ScpDamageHandler(attacker.ReferenceHub, damage, deathReason));
        }
    }
}