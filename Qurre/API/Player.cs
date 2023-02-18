using Mirror;
using Qurre.API.Controllers.Structs;
using RemoteAdmin;
using System.Collections.Generic;
using UnityEngine;
using Field = Qurre.Internal.Fields.Player;

namespace Qurre.API
{
    public class Player
    {
        internal bool Bot => false; // time field
        internal bool Invisible => false; // time field
        static public IEnumerable<Player> List => Field.Dictionary.Values;

        internal Player(GameObject gameObject) => new Player(ReferenceHub.GetHub(gameObject));
        internal Player(ReferenceHub _rh)
        {
            rh = _rh;
            go = _rh.gameObject;

            Disconnected = false;

            Broadcasts = new();

            Administrative = new(this);
            Client = new(this);
            Effects = new(this);
            GamePlay = new(this);
            HealthInfomation = new(this);
            Inventory = new(this);
            MovementState = new(this);
            PlayerStatsInfomation = new(this);
            RoleInfomation = new(this);
            UserInfomation = new(this);

            if (_rh.isLocalPlayer)
                return;

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
        public QueryProcessor QueryProcessor => rh.queryProcessor;
        public NetworkConnection Connection => IsHost ? rh.networkIdentity.connectionToServer : rh.networkIdentity.connectionToClient;
        public Transform Transform => rh.transform;

        public CommandSender Sender
        {
            get
            {
                if (IsHost) return ServerConsole.Scs;
                return QueryProcessor._sender;
            }
        }

        public int Ping => Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorServer.Peers[Connection.connectionId].Ping;
        public bool IsHost => rh.isLocalPlayer;
        public bool Disconnected { get; internal set; }
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

        public BroadcastsList Broadcasts { get; }

        public Classification.Player.Administrative Administrative { get; }
        public Classification.Player.Client Client { get; }
        public Classification.Player.EffectsManager Effects { get; }
        public Classification.Player.GamePlay GamePlay { get; }
        public Classification.Player.HealthInfomation HealthInfomation { get; }
        public Classification.Player.Inventory Inventory { get; }
        public Classification.Player.MovementState MovementState { get; }
        public Classification.Player.PlayerStatsInfomation PlayerStatsInfomation { get; }
        public Classification.Player.RoleInfomation RoleInfomation { get; }
        public Classification.Player.UserInfomation UserInfomation { get; }
    }
}