using System;
using JetBrains.Annotations;
using MapGeneration.Distributors;
using Mirror;
using Qurre.API.Addons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Generator
{
    private readonly Scp079Generator _generator;
    private readonly StructurePositionSync _positionSync;
    private string _name = string.Empty;

    internal Generator(Scp079Generator g)
    {
        _generator = g;
        _positionSync = _generator.GetComponent<StructurePositionSync>();
    }

    public Generator(Vector3 position, Quaternion? rotation = null)
    {
        if (Prefabs.Generator == null)
            throw new NullReferenceException(nameof(Prefabs.Generator));

        _generator = Object.Instantiate(Prefabs.Generator);

        _generator.transform.position = position;
        _generator.transform.rotation = rotation ?? new Quaternion();

        _positionSync = _generator.GetComponent<StructurePositionSync>();

        NetworkServer.Spawn(_generator.gameObject);

        _generator.netIdentity.UpdateData();

        Map.Generators.Add(this);
    }

    public GameObject GameObject => _generator.gameObject;
    public Transform Transform => GameObject.transform;

    public string Name
    {
        get => string.IsNullOrEmpty(_name) ? GameObject.name : _name;
        set => _name = value;
    }

    public Vector3 Position
    {
        get => Transform.position;
        set
        {
            _positionSync.Network_position = value;
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
            _positionSync.Network_rotationY = (sbyte)(value.eulerAngles.y / 5.625f);
            NetworkServer.UnSpawn(GameObject);
            Transform.rotation = value;
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

    public bool Open
    {
        get => _generator.HasFlag(_generator._flags, Scp079Generator.GeneratorFlags.Open);
        set
        {
            _generator.ServerSetFlag(Scp079Generator.GeneratorFlags.Open, value);
            _generator._targetCooldown = _generator._doorToggleCooldownTime;
        }
    }

    public bool Lock
    {
        get => !_generator.HasFlag(_generator._flags, Scp079Generator.GeneratorFlags.Unlocked);
        set
        {
            _generator.ServerSetFlag(Scp079Generator.GeneratorFlags.Unlocked, !value);
            _generator._targetCooldown = _generator._unlockCooldownTime;
        }
    }

    public bool Active
    {
        get => _generator.Activating;
        set
        {
            _generator.Activating = value;
            if (value) _generator._leverStopwatch.Restart();
            _generator._targetCooldown = _generator._doorToggleCooldownTime;
        }
    }

    public bool Engaged
    {
        get => _generator.Engaged;
        set => _generator.Engaged = value;
    }

    public short Time
    {
        get => _generator._syncTime;
        set => _generator.Network_syncTime = value;
    }

    public void Destroy()
    {
        Map.Generators.Remove(this);
        NetworkServer.Destroy(GameObject);
    }
}