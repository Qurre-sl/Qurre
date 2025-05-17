using System;
using JetBrains.Annotations;
using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.Subroutines;

namespace Qurre.API.Classification.Roles;

[PublicAPI]
public sealed class Scp106
{
    private readonly Controllers.Player _pl;

    internal Scp106(Controllers.Player pl)
    {
        _pl = pl;

        if (_pl.ReferenceHub.roleManager.CurrentRole is not Scp106Role roleBase)
            throw new NullReferenceException(nameof(roleBase));

        Base = roleBase;

        if (Subroutine.TryGetSubroutine(out Scp106Attack attack))
            Attack = attack;
        else
            Log.Debug("Null Debug: [Roles > Scp106] >> Scp106Attack is null");


        if (Subroutine.TryGetSubroutine(out Scp106SinkholeController sinkholeController))
            SinkholeController = sinkholeController;
        else
            Log.Debug("Null Debug: [Roles > Scp106] >> Scp106SinkholeController is null");


        if (Subroutine.TryGetSubroutine(out Scp106StalkAbility stalkAbility))
            StalkAbility = stalkAbility;
        else
            Log.Debug("Null Debug: [Roles > Scp106] >> Scp106StalkAbility is null");
    }

    public Scp106Role Base { get; }

    public bool IsWork => Base != null;

    public Scp106Attack? Attack { get; }
    public Scp106SinkholeController? SinkholeController { get; }
    public Scp106StalkAbility? StalkAbility { get; }

    public SubroutineManagerModule Subroutine
        => Base.SubroutineModule;
}