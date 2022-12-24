using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API.Classification.Player
{
    public class HealthInfomation
    {
        public Qurre.API.Player PlayerAPI;
        public float Health
        {
            get => ((HealthStat)PlayerAPI.ReferenceHub.playerStats.StatModules[0]).CurValue;
            set => ((HealthStat)PlayerAPI.ReferenceHub.playerStats.StatModules[0]).CurValue = value;
        }
        public float MaxHealth
        {
            get => ((HealthStat)PlayerAPI.ReferenceHub.playerStats.StatModules[0]).MaxValue;
        }
        public float ArtificialHealth
        {
            get => ((AhpStat)PlayerAPI.ReferenceHub.playerStats.StatModules[0]).CurValue;
            set => ((AhpStat)PlayerAPI.ReferenceHub.playerStats.StatModules[0]).CurValue = value;
        }
        public float MaxArtificalHealth
        {
            get => ((AhpStat)PlayerAPI.ReferenceHub.playerStats.StatModules[1]).MaxValue;
        }
    }
}
