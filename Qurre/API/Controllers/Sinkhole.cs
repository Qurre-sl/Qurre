using System;
using Hazards;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Sinkhole
{
    internal Sinkhole(SinkholeEnvironmentalHazard hole)
    {
        EnvironmentalHazard = hole;
    }

    public SinkholeEnvironmentalHazard EnvironmentalHazard { get; }

    public bool ImmunityScps { get; set; }

    public GameObject GameObject => EnvironmentalHazard.gameObject;
    public Transform Transform => GameObject.transform;
    public string Name => GameObject.name;

    public Vector3 Position
    {
        get => Transform.position;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            Transform.position = value;
            NetworkServer.Spawn(GameObject);
        }
    }

    public Quaternion Rotation
    {
        get => Transform.localRotation;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            Transform.localRotation = value;
            NetworkServer.Spawn(GameObject);
        }
    }

    public Vector3 Scale
    {
        get => Transform.localScale;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            Transform.localScale = value;
            NetworkServer.Spawn(GameObject);
        }
    }

    public float MaxDistance
    {
        get => EnvironmentalHazard.MaxDistance;
        set => EnvironmentalHazard.MaxDistance = value;
    }

    public float MaxHeightDistance
    {
        get => EnvironmentalHazard.MaxHeightDistance;
        set => EnvironmentalHazard.MaxHeightDistance = value;
    }

    public static bool operator ==(Sinkhole? first, Sinkhole? next)
    {
        return first?.GameObject == next?.GameObject;
    }

    public static bool operator !=(Sinkhole first, Sinkhole next)
    {
        return !(first == next);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Sinkhole sinkhole)
            return this == sinkhole;

        return false;
    }

    public override int GetHashCode()
    {
        return Tuple.Create(this, GameObject).GetHashCode();
    }
}