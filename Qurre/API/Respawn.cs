using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API
{
    static public class Respawn
    {
        public static float NtfTickets
        {
            get => RespawnTokensManager.Counters[1].Amount;
            set => RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, value);
        }
        public static float ChaosTickets
        {
            get => RespawnTokensManager.Counters[0].Amount;
            set => RespawnTokensManager.GrantTokens(SpawnableTeamType.ChaosInsurgency, value);
        }
        public static SpawnableTeamType NextKnownTeam => RespawnManager.Singleton.NextKnownTeam;
    }
}
