using Mirror;
using Qurre.API.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Field = Qurre.Internal.Fields.Player;

namespace Qurre.API
{
    public class Player
    {
        public readonly bool Bot = false;//time field
        public static IEnumerable<Player> List => Internal.Fields.Player.Dictionary.Values.Where(x => !x.Bot);

        internal Player(GameObject gameObject) => new Player(ReferenceHub.GetHub(gameObject));
        internal Player(ReferenceHub _rh)
        {
            rh = _rh;
            go = _rh.gameObject;

            UserInfomation = new(this);
            RoleInfomation = new(this);
            HealthInfomation = new(this);
            PlayerStatsInfomation = new(this);
            MovementState = new(this);
            GamePlay = new(this);

            if (!Field.Dictionary.ContainsKey(go)) Field.Dictionary.Add(go, this);
            else Field.Dictionary[go] = this;
        }

        private readonly ReferenceHub rh;
        private readonly GameObject go;
        private string _tag = "";

        public GameObject GameObject
        {
            get
            {
                if (rh is null || rh.gameObject is null) return go;
                else return rh.gameObject;
            }
        }
        public ReferenceHub ReferenceHub => rh;
        public CharacterClassManager ClassManager => rh.characterClassManager;
        public NetworkConnection Connection => IsHost ? rh.networkIdentity.connectionToServer : rh.networkIdentity.connectionToClient;

        public bool IsHost => rh.isLocalPlayer;
        public bool FriendlyFire { get; set; }
        public string Tag
        {
            get => _tag;
            set
            {
                if (value is null) return;
                _tag = value;
            }
        }

        public Classification.Player.UserInfomation UserInfomation { get; }
        public Classification.Player.RoleInfomation RoleInfomation { get; }
        public Classification.Player.HealthInfomation HealthInfomation { get; }
        public Classification.Player.PlayerStatsInfomation PlayerStatsInfomation { get; }
        public Classification.Player.MovementState MovementState { get; }
        public Classification.Player.GamePlay GamePlay { get; }
    }
}