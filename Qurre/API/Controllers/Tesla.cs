using System.Collections.Generic;
using JetBrains.Annotations;
using Mirror;
using PlayerRoles;
using UnityEngine;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Tesla
{
    private string _name;

    internal Tesla(TeslaGate gate)
    {
        _name = string.Empty;
        Gate = gate;
    }

    public TeslaGate Gate { get; }

    public bool Enable { get; set; } = true;
    public bool Allow079Interact { get; set; } = true;

    public List<RoleTypeId> ImmunityRoles { get; } = [];
    public List<Player> ImmunityPlayers { get; } = [];

    public GameObject GameObject => Gate.gameObject;
    public Transform Transform => GameObject.transform;

    public Vector3 Position => Transform.position;
    public Quaternion Rotation => Transform.localRotation;

    public Vector3 Scale
    {
        get => Transform.localScale;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            Transform.localScale = value;
            SizeOfKiller = value;
            NetworkServer.Spawn(GameObject);
        }
    }

    public Vector3 SizeOfKiller
    {
        get => Gate.sizeOfKiller;
        set => Gate.sizeOfKiller = value;
    }

    public string Name
    {
        get => string.IsNullOrEmpty(_name) ? GameObject.name : _name;
        set => _name = value;
    }

    public bool InProgress
    {
        get => Gate.InProgress;
        set => Gate.InProgress = value;
    }

    public float SizeOfTrigger
    {
        get => Gate.sizeOfTrigger;
        set => Gate.sizeOfTrigger = value;
    }

    public void Trigger(bool instant = false)
    {
        if (instant) Gate.RpcInstantBurst();
        else Gate.RpcPlayAnimation();
    }

    public void Destroy()
    {
        Object.Destroy(Gate.gameObject);
    }
}