using System;
using AdminToys;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Objects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class ShootingTarget
{
    public ShootingTarget(TargetPrefabs type, Vector3 position, Quaternion rotation = default, Vector3 size = default)
    {
        if (!type.GetPrefab().TryGetComponent<AdminToyBase>(out AdminToyBase? primitiveToyBase))
            throw new ArgumentNullException(nameof(primitiveToyBase));

        AdminToyBase? prim = Object.Instantiate(primitiveToyBase, position, rotation);

        Type = type;
        Base = (AdminToys.ShootingTarget)prim;
        Base.transform.localScale = size == default ? Vector3.one : size;
        NetworkServer.Spawn(Base.gameObject);

        Map.ShootingTargets.Add(this);
    }

    internal ShootingTarget(AdminToys.ShootingTarget @base)
    {
        try
        {
            Type = (TargetPrefabs)Enum.Parse(typeof(TargetPrefabs), @base._targetName);
        }
        catch
        {
            Type = TargetPrefabs.Binary;
        }

        Base = @base;

        Map.ShootingTargets.Add(this);
    }

    public TargetPrefabs Type { get; }

    public AdminToys.ShootingTarget Base { get; }

    public Vector3 Position
    {
        get => Base.transform.position;
        set
        {
            NetworkServer.UnSpawn(Base.gameObject);
            Base.transform.position = value;
            NetworkServer.Spawn(Base.gameObject);
        }
    }

    public Vector3 Scale
    {
        get => Base.transform.localScale;
        set
        {
            NetworkServer.UnSpawn(Base.gameObject);
            Base.transform.localScale = value;
            NetworkServer.Spawn(Base.gameObject);
        }
    }

    public Quaternion Rotation
    {
        get => Base.transform.localRotation;
        set
        {
            NetworkServer.UnSpawn(Base.gameObject);
            Base.transform.localRotation = value;
            NetworkServer.Spawn(Base.gameObject);
        }
    }

    public void Clear()
    {
        Base.ClearTarget();
    }

    public void Destroy()
    {
        NetworkServer.Destroy(Base.gameObject);
        Map.ShootingTargets.Remove(this);
    }
}