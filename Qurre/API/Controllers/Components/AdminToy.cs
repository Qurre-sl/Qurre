using AdminToys;
using JetBrains.Annotations;
using UnityEngine;

namespace Qurre.API.Controllers.Components;

[PublicAPI]
public abstract class AdminToy<TAdminToy> where TAdminToy : AdminToyBase
{
    public TAdminToy Base { get; protected init; } = null!;

    public GameObject GameObject => Base.gameObject;

    public byte MovementSmoothing
    {
        get => Base.NetworkMovementSmoothing;
        set => Base.NetworkMovementSmoothing = value;
    }

    public virtual bool IsStatic
    {
        get => Base.NetworkIsStatic;
        set
        {
            Base.NetworkIsStatic = value;
            if (!value) return;
            Base.NetworkPosition = Base.transform.position;
            Base.NetworkRotation = Base.transform.rotation;
            Base.NetworkScale = Base.transform.lossyScale;
        }
    }

    public Vector3 Position
    {
        get => Base.transform.position;
        set
        {
            Base.transform.position = value;
            Base.NetworkPosition = value;
        }
    }

    public Quaternion Rotation
    {
        get => Base.transform.rotation;
        set
        {
            Base.transform.rotation = value;
            Base.NetworkRotation = value;
        }
    }

    public Vector3 RotationEuler
    {
        get => Rotation.eulerAngles;
        set => Rotation = Quaternion.Euler(value);
    }

    public Vector3 Scale
    {
        get => Base.transform.localScale;
        set
        {
            Base.transform.localScale = value;
            Base.NetworkScale = Base.transform.lossyScale;
        }
    }

    public Vector3 GlobalScale => Base.transform.lossyScale;


    public abstract void Destroy();
}