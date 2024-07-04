using PlayerRoles.PlayableScps.Scp173;
using PlayerRoles.Subroutines;
using System.Collections.Generic;

namespace Qurre.API.Classification.Roles
{
    using Qurre.API;

    public sealed class Scp173
    {
        static public HashSet<Player> IgnoredPlayers { get; } = new();


        public Scp173Role Base { get; }

        public bool IsWork => pl.RoleInformation.Role is PlayerRoles.RoleTypeId.Scp173;

        public Scp173ObserversTracker Observers { get; }

        public SubroutineManagerModule Subroutine => Base.SubroutineModule;


        private readonly Player pl;
        internal Scp173(Player _pl)
        {
            pl = _pl;

            Base = pl.ReferenceHub.roleManager.CurrentRole as Scp173Role;

            if (Base is null)
                return;

            if (Subroutine.TryGetSubroutine(out Scp173ObserversTracker observers))
                Observers = observers;
            else
                Log.Debug($"Null Debug: [Roles > Scp106] >> Scp106Attack is null");
        }
    }
}