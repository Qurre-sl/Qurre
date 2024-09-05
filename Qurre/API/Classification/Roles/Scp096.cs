namespace Qurre.API.Classification.Roles;

using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.Subroutines;
using Qurre.API;

public sealed class Scp096
{
    public Scp096Role Base { get; }

    public bool IsWork => pl.RoleInformation.Role is PlayerRoles.RoleTypeId.Scp096;

    public Scp096RageManager RageManager { get; }
    public Scp096TargetsTracker TargetsTracker { get; }

    public SubroutineManagerModule Subroutine
        => Base.SubroutineModule;


    private readonly Player pl;
    internal Scp096(Player _pl)
    {
        pl = _pl;

        Base = pl.ReferenceHub.roleManager.CurrentRole as Scp096Role;

        if (Base is null)
            return;


        if (Subroutine.TryGetSubroutine(out Scp096RageManager rageManager))
            RageManager = rageManager;
        else
            Log.Debug($"Null Debug: [Roles > Scp096] >> Scp096RageManager is null");

        if (Subroutine.TryGetSubroutine(out Scp096TargetsTracker targetsTracker))
            TargetsTracker = targetsTracker;
        else
            Log.Debug($"Null Debug: [Roles > Scp096] >> Scp096TargetsTracker is null");
    }
}