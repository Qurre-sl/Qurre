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

    public Primitive(PrimitiveType type, Vector3 position = default, Color color = default,
        Quaternion rotation = default, Vector3 size = default, bool collider = true)
    {
        if (Prefabs.Primitive == null)
            throw new NullReferenceException(nameof(Prefabs.Primitive));

        if (!Prefabs.Primitive.TryGetComponent<AdminToyBase>(out AdminToyBase? primitiveToyBase))
            throw new NullReferenceException("AdminToyBase not found");

        AdminToyBase prim = Object.Instantiate(primitiveToyBase, position, rotation);

        Base = (PrimitiveObjectToy)prim;
        Base.SpawnerFootprint = new Footprint(Server.Host.ReferenceHub);
        NetworkServer.Spawn(Base.gameObject);

        Base.NetworkPrimitiveType = type;
        Base.NetworkMaterialColor = color == default ? Color.white : color;
        Base.NetworkPrimitiveFlags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible;

        Base.transform.localPosition = position;
        Base.transform.localRotation = rotation;
        Base.transform.localScale = size == default ? Vector3.one : size;

        Base.NetworkScale = Base.transform.lossyScale;
        Base.NetworkPosition = Base.transform.position;
        Base.NetworkRotation = new LowPrecisionQuaternion(Base.transform.rotation);

        Collider = collider;
        Map.Primitives.Add(this);
        NonStaticPrims.Add(this);
    }

    public PrimitiveObjectToy Base { get; }

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
                Base.NetworkRotation = new LowPrecisionQuaternion(Base.transform.rotation);
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

    public Vector3 Position
    {
        get => Base.transform.position;
        set
        {
            Base.transform.position = value;
            Base.NetworkPosition = Base.transform.position;
        }
    }

    public Quaternion Rotation
    {
        get => Base.transform.rotation;
        set
        {
            Base.transform.rotation = value;
            Base.NetworkRotation = new LowPrecisionQuaternion(Base.transform.rotation);
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

    public Vector3 GlobalScale
        => Base.transform.lossyScale;

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

    public PrimitiveFlags Flags
    {
        get => Base.NetworkPrimitiveFlags;
        set => Base.NetworkPrimitiveFlags = value;
    }

    public Color Color
    {
        get => Base.MaterialColor;
        set => Base.NetworkMaterialColor = value;
    }

    public PrimitiveType Type
    {
        get => Base.PrimitiveType;
        set => Base.NetworkPrimitiveType = value;
    }

    public void Destroy()
    {
        NetworkServer.Destroy(Base.gameObject);
        Map.Primitives.Remove(this);
        NonStaticPrims.Remove(this);
    }
}