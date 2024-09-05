namespace Qurre.API;

using CentralAuth;
using Mirror;
using Qurre.API.Addons;
using Qurre.API.Controllers.Structs;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using UnityEngine;
using Field = Internal.Fields.Player;

public class Player
{
    internal bool Bot => false; // time field
    internal bool Invisible => false; // time field
    static public IEnumerable<Player> List => Field.Dictionary.Values;

    internal Player(ReferenceHub _rh)
    {
        rh = _rh;
        go = _rh.gameObject;

        Disconnected = false;
        LastSynced = Time.time;
        JoinedTime = DateTime.Now;
        SpawnedTime = DateTime.Now;

        Variables = new();

        Broadcasts = new();

        Administrative = new(this);
        Client = new(this);
        Effects = new(this);
        GamePlay = new(this);
        HealthInformation = new(this);
        Inventory = new(this);
        MovementState = new(this);
        StatsInformation = new(this);
        RoleInformation = new(this);
        UserInformation = new(this);

        if (_rh.isLocalPlayer)
            return;

        if (!Field.Dictionary.ContainsKey(go))
            Field.Dictionary.Add(go, this);
        else
            Field.Dictionary[go] = this;

        if (!Field.Hubs.ContainsKey(rh))
            Field.Hubs.Add(rh, this);
        else
            Field.Hubs[rh] = this;
    }

    private readonly ReferenceHub rh;
    private readonly GameObject go;
    private string _tag = "";

    public GameObject GameObject
    {
        get
        {
            if (rh is null || rh.gameObject is null)
                return go;

            return rh.gameObject;
        }
    }
    public ReferenceHub ReferenceHub => rh;
    public PlayerAuthenticationManager AuthManager => rh.authManager;
    public CharacterClassManager ClassManager => rh.characterClassManager;
    public QueryProcessor QueryProcessor => rh.queryProcessor;
    public NetworkConnectionToClient ConnectionToClient => rh.networkIdentity.connectionToClient;
    public NetworkConnection Connection => IsHost ? rh.networkIdentity.connectionToServer : rh.networkIdentity.connectionToClient;
    public Transform Transform => rh.transform;
    public Transform CameraTransform => rh.PlayerCameraReference;

    public CommandSender Sender
    {
        get
        {
            if (IsHost)
                return ServerConsole.Scs;

            return QueryProcessor._sender;
        }
    }

    public int Ping => Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorServer.Peers[Connection.connectionId].Ping;
    public bool IsHost => rh.isLocalPlayer;
    public bool Disconnected { get; internal set; }
    public bool FriendlyFire { get; set; }
    public float LastSynced { get; internal set; }
    public DateTime JoinedTime { get; internal set; }
    public DateTime SpawnedTime { get; internal set; }
    public VariableDictionary<string, object> Variables { get; }
    public string Tag
    {
        get => _tag;
        set
        {
            if (value is null)
                return;

            _tag = value;
        }
    }

    public BroadcastsList Broadcasts { get; }

    public Classification.Player.Administrative Administrative { get; }
    public Classification.Player.Client Client { get; }
    public Classification.Player.EffectsManager Effects { get; }
    public Classification.Player.GamePlay GamePlay { get; }
    public Classification.Player.HealthInformation HealthInformation { get; }
    public Classification.Player.Inventory Inventory { get; }
    public Classification.Player.MovementState MovementState { get; }
    public Classification.Player.StatsInformation StatsInformation { get; }
    public Classification.Player.RoleInformation RoleInformation { get; }
    public Classification.Player.UserInformation UserInformation { get; }
}