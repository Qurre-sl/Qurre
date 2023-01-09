using PlayerStatsSystem;
namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    public class HealthInfomation
    {
        private readonly Player _player;
        internal HealthInfomation(Player pl) => _player = pl;

        public float Health
        {
            get => ((HealthStat)_player.ReferenceHub.playerStats.StatModules[0]).CurValue;
            set => ((HealthStat)_player.ReferenceHub.playerStats.StatModules[0]).CurValue = value;
        }
        public float MaxHealth
        {
            get => ((HealthStat)_player.ReferenceHub.playerStats.StatModules[0]).MaxValue;
        }
        public float ArtificialHealth
        {
            get => ((AhpStat)_player.ReferenceHub.playerStats.StatModules[0]).CurValue;
            set => ((AhpStat)_player.ReferenceHub.playerStats.StatModules[0]).CurValue = value;
        }
        public float MaxArtificalHealth
        {
            get => ((AhpStat)_player.ReferenceHub.playerStats.StatModules[1]).MaxValue;
        }
    }
}