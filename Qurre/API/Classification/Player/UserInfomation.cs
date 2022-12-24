using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API.Classification.Player
{
    public class UserInfomation
    {
        private Qurre.API.Player PlayerAPI;
        public string SteamId
        {
            get => PlayerAPI.ReferenceHub.characterClassManager.UserId;
            set => PlayerAPI.ReferenceHub.characterClassManager.UserId = value;
        }
        public string NickName
        {
            get => PlayerAPI.ReferenceHub.nicknameSync.Network_myNickSync;
            set => PlayerAPI.ReferenceHub.nicknameSync.Network_myNickSync = value;
        }
        public string DisplayName => PlayerAPI.ReferenceHub.nicknameSync.Network_displayName;
        public string CustomInfo => PlayerAPI.ReferenceHub.nicknameSync.Network_customPlayerInfoString;
        public string RoleName => PlayerAPI.ReferenceHub.serverRoles.Network_myText;
        public string RoleColor => PlayerAPI.ReferenceHub.serverRoles.Network_myColor;
    }
}