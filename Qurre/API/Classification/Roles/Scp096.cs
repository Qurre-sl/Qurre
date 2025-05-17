using System;
using JetBrains.Annotations;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.Subroutines;

namespace Qurre.API.Classification.Roles;

[PublicAPI]
public sealed class Scp096
{
    private readonly Controllers.Player _pl;

    internal Scp096(Controllers.Player pl)
    {
        _pl = pl;

        if (_pl.ReferenceHub.roleManager.CurrentRole is not Scp096Role roleBase)
            throw new NullReferenceException(nameof(roleBase));

        Base = roleBase;

        if (Subroutine.TryGetSubroutine(out Scp096RageManager rageManager))
            RageManager = rageManager;
        else
            Log.Debug("Null Debug: [Roles > Scp096] >> Scp096RageManager is null");

        if (Subroutine.TryGetSubroutine(out Scp096TargetsTracker targetsTracker))
            TargetsTracker = targetsTracker;
        else
            Log.Debug("Null Debug: [Roles > Scp096] >> Scp096TargetsTracker is null");
    }

    public Scp096Role Base { get; }

    public bool IsWork => Base != null;

    public Scp096RageManager? RageManager { get; }
    public Scp096TargetsTracker? TargetsTracker { get; }

    public SubroutineManagerModule Subroutine
        => Base.SubroutineModule;
}