using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PlayerRoles.PlayableScps.Scp173;
using PlayerRoles.Subroutines;

namespace Qurre.API.Classification.Roles;

[PublicAPI]
public sealed class Scp173
{
    private readonly Controllers.Player _pl;

    internal Scp173(Controllers.Player pl)
    {
        _pl = pl;

        if (_pl.ReferenceHub.roleManager.CurrentRole is not Scp173Role roleBase)
            throw new NullReferenceException(nameof(roleBase));

        Base = roleBase;

        if (Subroutine.TryGetSubroutine(out Scp173ObserversTracker observers))
            Observers = observers;
        else
            Log.Debug("Null Debug: [Roles > Scp173] >> Scp173ObserversTracker is null");
    }

    public static HashSet<Controllers.Player> IgnoredPlayers { get; } = [];

    public Scp173Role Base { get; }

    public bool IsWork => Base != null;

    public Scp173ObserversTracker? Observers { get; }

    public SubroutineManagerModule Subroutine
        => Base.SubroutineModule;
}