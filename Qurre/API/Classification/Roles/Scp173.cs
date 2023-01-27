namespace Qurre.API.Classification.Roles
{
    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.PlayableScps.Subroutines;
    using Qurre.API;
    using System.Collections.Generic;

    public sealed class Scp173
    {
        static public HashSet<Player> IgnoredPlayers { get; } = new();


        public Scp173Role Base { get; private set; }

        public bool IsWork => pl.RoleInfomation.Role == PlayerRoles.RoleTypeId.Scp173;

        public SubroutineManagerModule Subroutine => Base.SubroutineModule;


        private readonly Player pl;
        internal Scp173(Player _pl)
        {
            pl = _pl;

            Base = pl.ReferenceHub.roleManager.CurrentRole as Scp173Role;

            if (Base is null)
                return;
        }
    }
}