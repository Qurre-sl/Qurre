using Qurre.API.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Qurre.API
{
    public class Player
    {
        public static IEnumerable<Player> List => Internal.Fields.Player.Dictionary.Values.Where(x => !x.Bot);

        internal Player(GameObject gameObject) => new Player(ReferenceHub.GetHub(gameObject));
        internal Player(ReferenceHub _rh)
        {
            rh = _rh;
            go = _rh.gameObject;
            ui = _rh.characterClassManager.UserId;
            try { _nick = _rh.nicknameSync.Network_myNickSync; } catch { }

            UserInfomation = new(this);
            RoleInfomation = new(this);
            HealthInfomation = new(this);
        }

        private readonly ReferenceHub rh;
        private readonly GameObject go;
        private string ui = "";
        private readonly string _nick = "";
        private string _tag = "";
        internal List<KillElement> _kills = new();

        public ReferenceHub ReferenceHub => rh;

        public Classification.Player.UserInfomation UserInfomation { get; }
        public Classification.Player.RoleInfomation RoleInfomation { get; }
        public Classification.Player.HealthInfomation HealthInfomation { get; }
    }
}