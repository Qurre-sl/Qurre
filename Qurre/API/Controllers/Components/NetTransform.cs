using System;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

namespace Qurre.API.Controllers.Components;

[PublicAPI]
public abstract class NetTransform
{
    public abstract GameObject GameObject { get; }
    public Transform Transform => GameObject.transform;


    public Vector3 LocalPosition
    {
        get => Transform.localPosition;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            Transform.localPosition = value;
            NetworkServer.Spawn(GameObject);

            OnPositionUpdate?.Invoke();
        }
    }

    public Quaternion LocalRotation
    {
        get => Transform.localRotation;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            Transform.localRotation = value;
            NetworkServer.Spawn(GameObject);

            OnRotationUpdate?.Invoke();
        }
    }


    public Vector3 Position
    {
        get => Transform.position;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            Transform.position = value;
            NetworkServer.Spawn(GameObject);

            OnPositionUpdate?.Invoke();
        }
    }

    public Quaternion Rotation
    {
        get => Transform.rotation;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            Transform.rotation = value;
            NetworkServer.Spawn(GameObject);

            OnRotationUpdate?.Invoke();
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

            OnScaleUpdate?.Invoke();
        }
    }


    public event Action? OnPositionUpdate;
    public event Action? OnRotationUpdate;
    public event Action? OnScaleUpdate;


    public abstract void Destroy();
}