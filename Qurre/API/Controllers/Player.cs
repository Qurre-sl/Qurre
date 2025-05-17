using System;
using System.Collections.Generic;
using CentralAuth;
using JetBrains.Annotations;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using Qurre.API.Addons;
using Qurre.API.Classification.Player;
using Qurre.API.Controllers.Structs;
using RemoteAdmin;
using UnityEngine;
using Field = Qurre.Internal.Fields.Player;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Player
{
    private string _tag = string.Empty;

    internal Player(ReferenceHub rh)
    {
        ReferenceHub = rh;
        GameObject = rh.gameObject;

        Disconnected = false;
        LastSynced = Time.time;
        JoinedTime = DateTime.Now;
        SpawnedTime = DateTime.Now;

        Variables = [];

        Broadcasts = new BroadcastsList();

        Administrative = new Administrative(this);
        Client = new Client(this);
        Effects = new EffectsManager(this);
        GamePlay = new GamePlay(this);
        HealthInformation = new HealthInformation(this);
        Inventory = new Inventory(this);
        MovementState = new MovementState(this);
        StatsInformation = new StatsInformation(this);
        RoleInformation = new RoleInformation(this);
        UserInformation = new UserInformation(this);
        ServerSpecificSettings = new ServerSpecificSettings(this);

        if (rh.isLocalPlayer)
            return;

        Field.Dictionary[GameObject] = this;
        Field.Hubs[ReferenceHub] = this;
    }

    public static IEnumerable<Player> List => Field.Dictionary.Values;

    public GameObject GameObject { get; }
    public ReferenceHub ReferenceHub { get; }

    public PlayerAuthenticationManager AuthManager => ReferenceHub.authManager;
    public CharacterClassManager ClassManager => ReferenceHub.characterClassManager;
    public QueryProcessor QueryProcessor => ReferenceHub.queryProcessor;
    public NetworkConnectionToClient ConnectionToClient => ReferenceHub.networkIdentity.connectionToClient;

    public NetworkConnection Connection => IsHost
        ? ReferenceHub.networkIdentity.connectionToServer
        : ReferenceHub.networkIdentity.connectionToClient;

    public Transform Transform => ReferenceHub.transform;
    public Transform CameraTransform => ReferenceHub.PlayerCameraReference;

    public CommandSender Sender
    {
        get
        {
            if (IsHost)
                return ServerConsole.Scs;

            return QueryProcessor._sender;
        }
    }

    public int Ping => LiteNetLib4MirrorServer.Peers[Connection.connectionId].Ping;
    public bool IsHost => ReferenceHub.isLocalPlayer;
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
            if (string.IsNullOrEmpty(value))
                value = string.Empty;

            _tag = value;
        }
    }

    public BroadcastsList Broadcasts { get; }

    public Administrative Administrative { get; }
    public Client Client { get; }
    public EffectsManager Effects { get; }
    public GamePlay GamePlay { get; }
    public HealthInformation HealthInformation { get; }
    public Inventory Inventory { get; }
    public MovementState MovementState { get; }
    public StatsInformation StatsInformation { get; }
    public RoleInformation RoleInformation { get; }
    public UserInformation UserInformation { get; }
    public ServerSpecificSettings ServerSpecificSettings { get; }
}