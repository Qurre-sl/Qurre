using PlayerRoles;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API.Classification.Player
{
    public class RoleInfomation
    {
        private Qurre.API.Player PlayerAPI;
        public RoleTypeId Role
        {
            get => PlayerAPI.ReferenceHub.GetRoleId();
            set => PlayerAPI.ReferenceHub.roleManager.ServerSetRole(value, RoleChangeReason.RemoteAdmin);
        }
        public Team Team
        {
            get => PlayerAPI.ReferenceHub.GetTeam();
        }
        public float TimeForNextSequence
        {
            get => RespawnManager.Singleton._timeForNextSequence;
        }
    }
}
