using Mirror;
using PlayerRoles;
using Qurre.API.Addons;
using Qurre.API.Controllers.Structs;
using RemoteAdmin;
using System;
using System.Collections;
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
        internal bool Bot => false; // time field
        static public IEnumerable<Player> List => Field.Dictionary.Values;

        internal Player(GameObject gameObject) => new Player(ReferenceHub.GetHub(gameObject));
        internal Player(ReferenceHub _rh)
        {
            rh = _rh;
            go = _rh.gameObject;

            Broadcasts = new();

            UserInfomation = new(this);
            InventoryInformation = new(this);
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
        public QueryProcessor QueryProcessor => rh.queryProcessor;
        public NetworkConnection Connection => IsHost ? rh.networkIdentity.connectionToServer : rh.networkIdentity.connectionToClient;

        public CommandSender Sender
        {
            get
            {
                if (IsHost) return ServerConsole.Scs;
                return QueryProcessor._sender;
            }
        }

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
        public static Dictionary<string, Player> ArgsPlayers { get; set; } = new();
        public static Dictionary<GameObject, Player> Dictionary { get; } = new();
        public static Dictionary<int, Player> IdPlayers = new();
        public BroadcastsList Broadcasts { get; }

        public Classification.Player.UserInfomation UserInfomation { get; }
        public Classification.Player.InventoryInformation InventoryInformation { get; }
        public Classification.Player.RoleInfomation RoleInfomation { get; }
        public Classification.Player.HealthInfomation HealthInfomation { get; }
        public Classification.Player.PlayerStatsInfomation PlayerStatsInfomation { get; }
        public Classification.Player.MovementState MovementState { get; }
        public Classification.Player.GamePlay GamePlay { get; }
        public Classification.Player.Client Client { get;}
        public Classification.Player.InventoryItems InventoryItems { get; }
        public static IEnumerable<Player> Get(Team team) => List.Where(player => player.RoleInfomation.Team == team);
        public static IEnumerable<Player> Get(RoleTypeId role) => List.Where(player => player.RoleInfomation.Role == role);
        public static Player Get(CommandSender sender) => sender == null ? null : Get(sender.SenderId);
        public static Player Get(ReferenceHub referenceHub) => referenceHub == null ? null : Get(referenceHub.gameObject);
        public static Player Get(GameObject gameObject)
        {
            if (gameObject == null) return null;
            Dictionary.TryGetValue(gameObject, out Player player);
            return player;
        }
        public static Player Get(uint netId) => ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub) ? Get(hub) : null;
        public static Player Get(int playerId)
        {
            if (IdPlayers.ContainsKey(playerId)) return IdPlayers[playerId];
            foreach (Player pl in List.Where(x => x.UserInfomation.Id == playerId))
            {
                IdPlayers.Add(playerId, pl);
                return pl;
            }
            return null;
        }
        public static Player Get(string args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(args))
                    return null;
                if (ArgsPlayers.TryGetValue(args, out Player playerFound) && playerFound?.ReferenceHub != null)
                    return playerFound;
                if (int.TryParse(args, out int id))
                    return Get(id);
                if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") || args.EndsWith("@patreon"))
                {
                    foreach (Player player in Dictionary.Values)
                    {
                        if (player.UserInfomation.UserId == args)
                        {
                            playerFound = player;
                            break;
                        }
                    }
                }
                else
                {
                    int lastnameDifference = 31;
                    string firstString = args.ToLower();
                    foreach (Player player in Dictionary.Values)
                    {
                        if (player.UserInfomation.Nickname == null) continue;
                        if (!player.UserInfomation.Nickname.Contains(args))
                            continue;
                        string secondString = player.UserInfomation.Nickname;
                        int nameDifference = secondString.Length - firstString.Length;
                        if (nameDifference < lastnameDifference)
                        {
                            lastnameDifference = nameDifference;
                            playerFound = player;
                        }
                    }
                }
                if (playerFound != null)
                    ArgsPlayers[args] = playerFound;
                return playerFound;
            }
            catch (Exception ex)
            {
                Log.Error($"[API.Player.Get(string)] umm, error: {ex}");
                return null;
            }
        }
    }
}