using PlayerRoles.PlayableScps.Scp079;

namespace Qurre.API.Classification.Roles
{
    using PlayerRoles.PlayableScps.Subroutines;
    using Qurre.API;
    using Qurre.API.Controllers;
    using System.Linq;

    public sealed class Scp079
    {
        public Scp079Role Base { get; private set; }

        public bool IsWork => pl.RoleInfomation.Role == PlayerRoles.RoleTypeId.Scp079;

        public Scp079AbilityBase Ability { get; private set; }
        public Scp079AuxManager AuxManager { get; private set; }
        public Scp079BlackoutRoomAbility BlackoutRoomAbility { get; private set; }
        public Scp079BlackoutZoneAbility BlackoutZoneAbility { get; private set; }
        public Scp079DoorAbility DoorAbility { get; private set; }
        public Scp079DoorLockChanger DoorLockChanger { get; private set; }
        public Scp079DoorLockReleaser DoorLockReleaser { get; private set; }
        public Scp079DoorStateChanger DoorStateChanger { get; private set; }
        public Scp079ElevatorStateChanger ElevatorStateChanger { get; private set; }
        public Scp079KeyAbilityBase KeyAbilityBase { get; private set; }
        public Scp079LockdownRoomAbility LockdownRoomAbility { get; private set; }
        public Scp079LostSignalHandler LostSignalHandler { get; private set; }
        public Scp079SpeakerAbility SpeakerAbility { get; private set; }
        public Scp079TeslaAbility TeslaAbility { get; private set; }
        public Scp079TierManager TierManager { get; private set; }

        public SubroutineManagerModule Subroutine => Base.SubroutineModule;

        public Camera Camera
        {
            get => Map.Cameras.FirstOrDefault(x => x.Base == Base.CurrentCamera);
            set => Base._curCamSync.CurrentCamera = value.Base;
        }

        public int Lvl
        {
            get => TierManager.AccessTierIndex + 1;
            set => TierManager.AccessTierIndex = value - 1;
        }
        public int Exp
        {
            get => TierManager.TotalExp;
            set => TierManager.TotalExp = value;
        }

        public float Energy
        {
            get => AuxManager.CurrentAux;
            set => AuxManager.CurrentAux = value;
        }
        public float MaxEnergy
        {
            get => AuxManager._maxPerTier[TierManager.AccessTierIndex];
            set => AuxManager._maxPerTier[TierManager.AccessTierIndex] = value;
        }


        public void LostSignal(float dur)
            => LostSignalHandler.ServerLoseSignal(dur);


        private readonly Player pl;
        internal Scp079(Player _pl)
        {
            pl = _pl;

            Base = pl.ReferenceHub.roleManager.CurrentRole as Scp079Role;

            if (Base is null)
                return;

            if (Subroutine.TryGetSubroutine(out Scp079AbilityBase ability))
                Ability = ability;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> Ability is null");

            if (Subroutine.TryGetSubroutine(out Scp079AuxManager auxManager))
                AuxManager = auxManager;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> AuxManager is null");

            if (Subroutine.TryGetSubroutine(out Scp079BlackoutRoomAbility blackoutRoomAbility))
                BlackoutRoomAbility = blackoutRoomAbility;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> BlackoutRoomAbility is null");

            if (Subroutine.TryGetSubroutine(out Scp079BlackoutZoneAbility blackoutZoneAbility))
                BlackoutZoneAbility = blackoutZoneAbility;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> BlackoutZoneAbility is null");

            if (Subroutine.TryGetSubroutine(out Scp079DoorAbility doorAbility))
                DoorAbility = doorAbility;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> DoorAbility is null");

            if (Subroutine.TryGetSubroutine(out Scp079DoorLockChanger doorLockChanger))
                DoorLockChanger = doorLockChanger;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> DoorLockChanger is null");

            if (Subroutine.TryGetSubroutine(out Scp079DoorLockReleaser doorLockReleaser))
                DoorLockReleaser = doorLockReleaser;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> DoorLockReleaser is null");

            if (Subroutine.TryGetSubroutine(out Scp079DoorStateChanger doorStateChanger))
                DoorStateChanger = doorStateChanger;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> DoorStateChanger is null");

            if (Subroutine.TryGetSubroutine(out Scp079ElevatorStateChanger elevatorStateChanger))
                ElevatorStateChanger = elevatorStateChanger;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> ElevatorStateChanger is null");

            if (Subroutine.TryGetSubroutine(out Scp079KeyAbilityBase keyAbilityBase))
                KeyAbilityBase = keyAbilityBase;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> KeyAbilityBase is null");

            if (Subroutine.TryGetSubroutine(out Scp079LockdownRoomAbility lockdownRoomAbility))
                LockdownRoomAbility = lockdownRoomAbility;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> LockdownRoomAbility is null");

            if (Subroutine.TryGetSubroutine(out Scp079LostSignalHandler lostSignalHandler))
                LostSignalHandler = lostSignalHandler;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> LostSignalHandler is null");

            if (Subroutine.TryGetSubroutine(out Scp079SpeakerAbility speakerAbility))
                SpeakerAbility = speakerAbility;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> SpeakerAbility is null");

            if (Subroutine.TryGetSubroutine(out Scp079TeslaAbility teslaAbility))
                TeslaAbility = teslaAbility;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> TeslaAbility is null");

            if (Subroutine.TryGetSubroutine(out Scp079TierManager tierManager))
                TierManager = tierManager;
            else
                Log.Debug($"Null Debug: [Roles > Scp079] >> TierManager is null");
        }
    }
}