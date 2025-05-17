using System;
using Hazards;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Controllers.Components;
using Qurre.API.World;
using UnityEngine;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Sinkhole : NetTransform
{
    internal Sinkhole(SinkholeEnvironmentalHazard hole)
    {
        EnvironmentalHazard = hole;
    }

    public SinkholeEnvironmentalHazard EnvironmentalHazard { get; }

    public bool ImmunityScps { get; set; }

    public override GameObject GameObject => EnvironmentalHazard.gameObject;
    public string Name => GameObject.name;

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

    public override void Destroy()
    {
        NetworkServer.Destroy(GameObject);
        Map.Sinkholes.Remove(this);
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