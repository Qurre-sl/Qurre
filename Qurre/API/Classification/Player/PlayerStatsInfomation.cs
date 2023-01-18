using System.Collections.Generic;
using Qurre.API.Addons;

namespace Qurre.API.Classification.Player
{
    public class PlayerStatsInfomation
    {
        internal List<KillElement> _kills = new ();
        private readonly API.Player _player;

        internal PlayerStatsInfomation(API.Player pl)
        {
            _player = pl;
            DeathsCount = 0;
        }

        public IReadOnlyCollection<KillElement> Kills => _kills.AsReadOnly();
        public int KillsCount => _kills.Count;
        public int DeathsCount { get; internal set; }
    }
}