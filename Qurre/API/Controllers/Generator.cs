using System;
using JetBrains.Annotations;
using MapGeneration.Distributors;
using Mirror;
using Qurre.API.Addons;
using Qurre.API.Controllers.Components;
using Qurre.API.World;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Generator : NetTransform
{
    private readonly Scp079Generator _generator;
    private readonly StructurePositionSync _positionSync;
    private string _name = string.Empty;

    internal Generator(Scp079Generator g)
    {
        _generator = g;
        _positionSync = _generator.GetComponent<StructurePositionSync>();
        SetupActions();
    }

    public Generator(Vector3 position, Quaternion? rotation = null)
    {
        if (Prefabs.Generator == null)
            throw new NullReferenceException(nameof(Prefabs.Generator));

        _generator = Object.Instantiate(Prefabs.Generator);

        _generator.transform.position = position;
        _generator.transform.rotation = rotation ?? new Quaternion();

        _positionSync = _generator.GetComponent<StructurePositionSync>();

        SetupActions();
        NetworkServer.Spawn(_generator.gameObject);

        _generator.netIdentity.UpdateData();

        Map.Generators.Add(this);
    }

    public override GameObject GameObject => _generator.gameObject;

    public string Name
    {
        get => string.IsNullOrEmpty(_name) ? GameObject.name : _name;
        set => _name = value;
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

    private void SetupActions()
    {
        OnPositionUpdate += () => _positionSync.Network_position = Position;
        OnRotationUpdate += () => _positionSync.Network_rotationY = (sbyte)(Rotation.eulerAngles.y / 5.625f);
    }

    public override void Destroy()
    {
        NetworkServer.Destroy(GameObject);
        Map.Generators.Remove(this);
    }
}