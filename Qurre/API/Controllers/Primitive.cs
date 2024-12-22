using System;
using System.Collections.Generic;
using AdminToys;
using Footprinting;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Addons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Primitive
{
    internal static bool AllowStatic;
    internal static readonly HashSet<Primitive> CachedToSetStatic = [];
    internal static readonly HashSet<Primitive> NonStaticPrims = [];

    public PrimitiveObjectToy Base { get; }

    public Vector3 Position
    {
        get => Base.transform.position;
        set
        {
            Base.transform.position = value;
            Base.NetworkPosition = value;
        }
    }

    public Quaternion RotationQuaternion
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
        get => Base.transform.localRotation.eulerAngles;
        set
        {
            var quaternion = Quaternion.Euler(value);
            Base.transform.localRotation = quaternion;
            Base.NetworkRotation = quaternion;
        }
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

    public byte MovementSmoothing
    {
        get => Base.NetworkMovementSmoothing;
        set => Base.NetworkMovementSmoothing = value;
    }

    public bool IsStatic
    {
        get => Base.IsStatic;
        set
        {
            if (value)
            {
                Base.NetworkPosition = Base.transform.position;
                Base.NetworkRotation = Base.transform.rotation;
                Base.NetworkScale = Base.transform.lossyScale;
            }

            if (AllowStatic)
            {
                Base.NetworkIsStatic = value;

                if (value)
                    NonStaticPrims.Remove(this);
                else
                    NonStaticPrims.Add(this);

                if (!value && CachedToSetStatic.Contains(this))
                    CachedToSetStatic.Remove(this);
                return;
            }

            if (value)
                CachedToSetStatic.Add(this);
            else
                CachedToSetStatic.Remove(this);
        }
    }

    public Color Color
    {
        get => Base.NetworkMaterialColor;
        set => Base.NetworkMaterialColor = value;
    }

    public PrimitiveType Type
    {
        get => Base.NetworkPrimitiveType;
        set => Base.NetworkPrimitiveType = value;
    }

    public PrimitiveFlags Flags
    {
        get => Base.NetworkPrimitiveFlags;
        set => Base.NetworkPrimitiveFlags = value;
    }

    public bool Collider
    {
        get => Flags.HasFlag(PrimitiveFlags.Collidable);
        set
        {
            if (!value)
                Flags &= ~PrimitiveFlags.Collidable;
            else
                Flags |= PrimitiveFlags.Collidable;
        }
    }

    public bool Visible
    {
        get => Flags.HasFlag(PrimitiveFlags.Visible);
        set
        {
            if (!value)
                Flags &= ~PrimitiveFlags.Visible;
            else
                Flags |= PrimitiveFlags.Visible;
        }
    }

    public Primitive(PrimitiveType type, Vector3 position = default, Color color = default,
        Quaternion rotation = default, Vector3 size = default, bool collider = true,
        bool visible = true)
    {
        if (Prefabs.Primitive == null)
            throw new NullReferenceException(nameof(Prefabs.Primitive));

        if (!Prefabs.Primitive.TryGetComponent<PrimitiveObjectToy>(out var primitiveToyBase))
            throw new NullReferenceException("PrimitiveObjectToy not found");

        Base = Object.Instantiate(primitiveToyBase);
        Base.SpawnerFootprint = new Footprint(Server.Host.ReferenceHub);
        NetworkServer.Spawn(Base.gameObject);

        Position = position;
        RotationQuaternion = rotation;
        Scale = size == default ? Vector3.one : size;

        Type = type;
        Color = color == default ? Color.white : color;
        Collider = collider;
        Visible = visible;

        Map.Primitives.Add(this);
        NonStaticPrims.Add(this);
    }

    public void Destroy()
    {
        NetworkServer.Destroy(Base.gameObject);
        Map.Primitives.Remove(this);
        NonStaticPrims.Remove(this);
    }
}