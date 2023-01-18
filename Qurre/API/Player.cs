using System.Collections.Generic;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using Qurre.API.Classification.Player;
using Qurre.API.Controllers.Structs;
using RemoteAdmin;
using UnityEngine;
using Field = Qurre.Internal.Fields.Player;

namespace Qurre.API
{
    public class Player
    {
        private readonly GameObject go;
        private string _tag = "";

        internal Player(GameObject gameObject) => new Player(ReferenceHub.GetHub(gameObject));

        internal Player(ReferenceHub _rh)
        {
            ReferenceHub = _rh;
            go = _rh.gameObject;

            Broadcasts = new ();

            Administrative = new (this);
            Client = new (this);
            GamePlay = new (this);
            HealthInfomation = new (this);
            Inventory = new (this);
            MovementState = new (this);
            PlayerStatsInfomation = new (this);
            RoleInfomation = new (this);
            UserInfomation = new (this);

            if (_rh.isLocalPlayer)
            {
                return;
            }

            if (!Field.Dictionary.ContainsKey(go))
            {
                Field.Dictionary.Add(go, this);
            }
            else
            {
                Field.Dictionary[go] = this;
            }
        }

        public static IEnumerable<Player> List => Field.Dictionary.Values;

        public GameObject GameObject
        {
            get
            {
                if (ReferenceHub is null || ReferenceHub.gameObject is null)
                {
                    return go;
                }

                return ReferenceHub.gameObject;
            }
        }

        public ReferenceHub ReferenceHub { get; }

        public CharacterClassManager ClassManager => ReferenceHub.characterClassManager;
        public QueryProcessor QueryProcessor => ReferenceHub.queryProcessor;
        public NetworkConnection Connection => IsHost ? ReferenceHub.networkIdentity.connectionToServer : ReferenceHub.networkIdentity.connectionToClient;

        public CommandSender Sender
        {
            get
            {
                if (IsHost)
                {
                    return ServerConsole.Scs;
                }

                return QueryProcessor._sender;
            }
        }

        public int Ping => LiteNetLib4MirrorServer.Peers[Connection.connectionId].Ping;
        public bool IsHost => ReferenceHub.isLocalPlayer;
        public bool FriendlyFire { get; set; }

        public string Tag
        {
            get => _tag;
            set
            {
                if (value is null)
                {
                    return;
                }

                _tag = value;
            }
        }

        public BroadcastsList Broadcasts { get; }

        public Administrative Administrative { get; }
        public Client Client { get; }
        public GamePlay GamePlay { get; }
        public HealthInfomation HealthInfomation { get; }
        public Inventory Inventory { get; }
        public MovementState MovementState { get; }
        public PlayerStatsInfomation PlayerStatsInfomation { get; }
        public RoleInfomation RoleInfomation { get; }
        public UserInfomation UserInfomation { get; }
        internal bool Bot => false; // time field
    }
}