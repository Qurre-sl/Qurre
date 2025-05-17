using System;
using JetBrains.Annotations;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.Subroutines;
using Qurre.API.Controllers;
using Qurre.API.World;

namespace Qurre.API.Classification.Roles;

[PublicAPI]
public sealed class Scp079
{
    private readonly Controllers.Player _pl;

    internal Scp079(Controllers.Player pl)
    {
        _pl = pl;

        if (_pl.ReferenceHub.roleManager.CurrentRole is not Scp079Role roleBase)
            throw new NullReferenceException(nameof(roleBase));

        Base = roleBase;

        if (Subroutine.TryGetSubroutine(out Scp079AbilityBase ability))
            Ability = ability;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> Ability is null");

        if (Subroutine.TryGetSubroutine(out Scp079AuxManager auxManager))
            AuxManager = auxManager;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> AuxManager is null");

        if (Subroutine.TryGetSubroutine(out Scp079BlackoutRoomAbility blackoutRoomAbility))
            BlackoutRoomAbility = blackoutRoomAbility;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> BlackoutRoomAbility is null");

        if (Subroutine.TryGetSubroutine(out Scp079BlackoutZoneAbility blackoutZoneAbility))
            BlackoutZoneAbility = blackoutZoneAbility;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> BlackoutZoneAbility is null");

        if (Subroutine.TryGetSubroutine(out Scp079DoorAbility doorAbility))
            DoorAbility = doorAbility;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> DoorAbility is null");

        if (Subroutine.TryGetSubroutine(out Scp079DoorLockChanger doorLockChanger))
            DoorLockChanger = doorLockChanger;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> DoorLockChanger is null");

        if (Subroutine.TryGetSubroutine(out Scp079DoorLockReleaser doorLockReleaser))
            DoorLockReleaser = doorLockReleaser;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> DoorLockReleaser is null");

        if (Subroutine.TryGetSubroutine(out Scp079DoorStateChanger doorStateChanger))
            DoorStateChanger = doorStateChanger;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> DoorStateChanger is null");

        if (Subroutine.TryGetSubroutine(out Scp079ElevatorStateChanger elevatorStateChanger))
            ElevatorStateChanger = elevatorStateChanger;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> ElevatorStateChanger is null");

        if (Subroutine.TryGetSubroutine(out Scp079KeyAbilityBase keyAbilityBase))
            KeyAbilityBase = keyAbilityBase;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> KeyAbilityBase is null");

        if (Subroutine.TryGetSubroutine(out Scp079LockdownRoomAbility lockdownRoomAbility))
            LockdownRoomAbility = lockdownRoomAbility;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> LockdownRoomAbility is null");

        if (Subroutine.TryGetSubroutine(out Scp079LostSignalHandler lostSignalHandler))
            LostSignalHandler = lostSignalHandler;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> LostSignalHandler is null");

        if (Subroutine.TryGetSubroutine(out Scp079SpeakerAbility speakerAbility))
            SpeakerAbility = speakerAbility;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> SpeakerAbility is null");

        if (Subroutine.TryGetSubroutine(out Scp079TeslaAbility teslaAbility))
            TeslaAbility = teslaAbility;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> TeslaAbility is null");

        if (Subroutine.TryGetSubroutine(out Scp079TierManager tierManager))
            TierManager = tierManager;
        else
            Log.Debug("Null Debug: [Roles > Scp079] >> TierManager is null");
    }

    public Scp079Role Base { get; }

    public bool IsWork => Base != null;

    public Scp079AbilityBase? Ability { get; }
    public Scp079AuxManager? AuxManager { get; }
    public Scp079BlackoutRoomAbility? BlackoutRoomAbility { get; }
    public Scp079BlackoutZoneAbility? BlackoutZoneAbility { get; }
    public Scp079DoorAbility? DoorAbility { get; }
    public Scp079DoorLockChanger? DoorLockChanger { get; }
    public Scp079DoorLockReleaser? DoorLockReleaser { get; }
    public Scp079DoorStateChanger? DoorStateChanger { get; }
    public Scp079ElevatorStateChanger? ElevatorStateChanger { get; }
    public Scp079KeyAbilityBase? KeyAbilityBase { get; }
    public Scp079LockdownRoomAbility? LockdownRoomAbility { get; }
    public Scp079LostSignalHandler? LostSignalHandler { get; }
    public Scp079SpeakerAbility? SpeakerAbility { get; }
    public Scp079TeslaAbility? TeslaAbility { get; }
    public Scp079TierManager? TierManager { get; }

    public SubroutineManagerModule Subroutine
        => Base.SubroutineModule;

    public Camera Camera
    {
        get => Map.Cameras.Find(x => x.Base == Base.CurrentCamera);
        set => Base._curCamSync.CurrentCamera = value.Base;
    }

    public int Lvl
    {
        get => TierManager?.AccessTierIndex + 1 ?? 0;
        set
        {
            if (TierManager == null)
                return;

            TierManager.AccessTierIndex = value - 1;
        }
    }

    public int Exp
    {
        get => TierManager?.TotalExp ?? 0;
        set
        {
            if (TierManager == null)
                return;

            TierManager.TotalExp = value;
        }
    }

    public float Energy
    {
        get => AuxManager?.CurrentAux ?? 0;
        set
        {
            if (AuxManager == null)
                return;

            AuxManager.CurrentAux = value;
        }
    }

    public float MaxEnergy
    {
        get
        {
            if (AuxManager == null || TierManager == null)
                return 0;

            return AuxManager._maxPerTier[TierManager.AccessTierIndex];
        }
        set
        {
            if (AuxManager == null || TierManager == null)
                return;

            AuxManager._maxPerTier[TierManager.AccessTierIndex] = value;
        }
    }


    public void LostSignal(float dur)
    {
        LostSignalHandler?.ServerLoseSignal(dur);
    }
}