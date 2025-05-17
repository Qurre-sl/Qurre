using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CentralAuth;
using JetBrains.Annotations;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using PlayerRoles;
using Qurre.API.Addons;
using Qurre.API.Attributes;
using Qurre.API.Controllers.Structs;
using Qurre.API.Entities.Characters.Components;
using Qurre.Events;
using Qurre.Events.Structs;
using RemoteAdmin;
using UnityEngine;
using Field = Qurre.Internal.Fields.Player;

namespace Qurre.API.Entities.Characters;

[PublicAPI]
public class Player
{
    private Player(ReferenceHub referenceHub)
    {
        ReferenceHub = referenceHub;
        GameObject = referenceHub.gameObject;

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

        if (referenceHub.isLocalPlayer)
            return;

        Field.Dictionary[GameObject] = this;
        Field.Hubs[ReferenceHub] = this;
        Field.Ids[UserInformation.Id] = this;
    }

    public static IReadOnlyCollection<Player> List => Field.Dictionary.Values;

    public string Tag { get; set; } = string.Empty;

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

    /// <summary>
    /// Опредяет то, может ли игрок наносить урон своим союзникам.
    /// </summary>
    public bool AllowFriendlyDamage { get; set; }
    public float LastSynced { get; internal set; }
    public DateTime JoinedTime { get; internal set; }
    public DateTime SpawnedTime { get; private set; }
    public VariableDictionary<object> Variables { get; }

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

    public static Player? Get(ReferenceHub referenceHub)
    {
        if (!referenceHub) return null;
        return Field.Hubs.TryGetValue(referenceHub, out var player) ? player : new Player(referenceHub);
    }

    public static Player? Get(GameObject gameObject)
    {
        if (!gameObject) return null;
        if (Field.Dictionary.TryGetValue(gameObject, out var player))
            return player;

        var referenceHub = gameObject.GetComponent<ReferenceHub>();
        return Get(referenceHub);
    }

    public static Player? Get(int id)
    {
        return Field.Ids.GetValueOrDefault(id);
    }

    public static Player? Get(string args)
    {
        return Field.Args.GetValueOrDefault(args);
    }

    public static bool TryGet(ReferenceHub referenceHub, [NotNullWhen(true)] out Player? player)
    {
        player = Get(referenceHub);
        return player is not null;
    }

    public static bool TryGet(GameObject gameObject, [NotNullWhen(true)] out Player? player)
    {
        player = Get(gameObject);
        return player is not null;
    }

    public static bool TryGet(int id, [NotNullWhen(true)] out Player? player)
    {
        player = Get(id);
        return player is not null;
    }

    public static bool TryGet(string args, [NotNullWhen(true)] out Player? player)
    {
        player = Get(args);
        return player is not null;
    }

    [EventMethod(PlayerEvents.ChangeRole, int.MinValue)]
    private static void SetSpawnedTime(ChangeRoleEvent ev)
    {
        if (!ev.IsAllowed)
            return;

        if (ev.Role is RoleTypeId.Spectator or RoleTypeId.Overwatch or RoleTypeId.Filmmaker)
            return;

        ev.Player.SpawnedTime = DateTime.Now;
    }
}