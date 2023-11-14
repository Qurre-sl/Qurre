using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.Subroutines;

namespace Qurre.API.Classification.Roles
{
    using Qurre.API;

    public sealed class Scp106
    {
        public Scp106Role Base { get; private set; }

        public bool IsWork => pl.RoleInfomation.Role == PlayerRoles.RoleTypeId.Scp106;

        public SubroutineManagerModule Subroutine => Base.SubroutineModule;


        private readonly Player pl;
        internal Scp106(Player _pl)
        {
            pl = _pl;

            Base = pl.ReferenceHub.roleManager.CurrentRole as Scp106Role;

            if (Base is null)
                return;
        }
    }
}