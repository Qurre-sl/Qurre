﻿using System.Collections.Generic;
namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    using Qurre.API.Addons;

    public sealed class PlayerStatsInfomation
    {
        private readonly Player _player;
        internal PlayerStatsInfomation(Player pl)
        {
            _player = pl;
            DeathsCount = 0;
        }

        internal List<KillElement> _kills = new();

        public IReadOnlyCollection<KillElement> Kills => _kills.AsReadOnly();
        public int KillsCount => _kills.Count;
        public int DeathsCount { get; internal set; }
    }
}