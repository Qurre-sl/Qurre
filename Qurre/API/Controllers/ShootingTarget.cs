using System;
using AdminToys;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Controllers.Components;
using Qurre.API.Objects;
using Qurre.API.World;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class ShootingTarget : AdminToy<AdminToys.ShootingTarget>
{
    public ShootingTarget(TargetPrefabs type, Vector3 position, Quaternion rotation = default, Vector3 size = default)
    {
        if (!type.GetPrefab().TryGetComponent<AdminToyBase>(out AdminToyBase? primitiveToyBase))
            throw new ArgumentNullException(nameof(primitiveToyBase));

        AdminToyBase? prim = Object.Instantiate(primitiveToyBase, position, rotation);

        PrefabType = type;
        Base = (AdminToys.ShootingTarget)prim;
        Base.transform.localScale = size == default ? Vector3.one : size;
        NetworkServer.Spawn(Base.gameObject);

        Map.ShootingTargets.Add(this);
    }

    internal ShootingTarget(AdminToys.ShootingTarget @base)
    {
        try
        {
            PrefabType = (TargetPrefabs)Enum.Parse(typeof(TargetPrefabs), @base._targetName);
        }
        catch
        {
            PrefabType = TargetPrefabs.Binary;
        }

        Base = @base;

        Map.ShootingTargets.Add(this);
    }

    public TargetPrefabs PrefabType { get; }

    public void Clear()
    {
        Base.ClearTarget();
    }

    public override void Destroy()
    {
        NetworkServer.Destroy(Base.gameObject);
        Map.ShootingTargets.Remove(this);
    }
}