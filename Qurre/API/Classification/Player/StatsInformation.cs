using System.Collections.Generic;
using JetBrains.Annotations;
using Qurre.API.Addons;

namespace Qurre.API.Classification.Player;

[PublicAPI]
public sealed class StatsInformation
{
    private readonly Controllers.Player _player;

    internal List<KillElement> LocalKills = [];

    internal StatsInformation(Controllers.Player pl)
    {
        _player = pl;
        DeathsCount = 0;
    }

    public int DeathsCount { get; internal set; }

    public IReadOnlyCollection<KillElement> Kills
        => LocalKills.AsReadOnly();

    public int KillsCount
        => LocalKills.Count;
}