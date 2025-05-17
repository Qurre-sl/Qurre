using System;
using System.Collections.Generic;
using AdminToys;
using Footprinting;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Addons;
using Qurre.API.Controllers.Components;
using Qurre.API.World;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Primitive : AdminToy<PrimitiveObjectToy>
{
    internal static bool AllowStatic;
    internal static readonly HashSet<Primitive> CachedToSetStatic = [];
    internal static readonly HashSet<Primitive> NonStaticPrims = [];

    public Primitive(PrimitiveType type, Vector3 position = default, Color color = default,
        Quaternion rotation = default, Vector3 size = default, bool collider = true,
        bool visible = true)
    {
        if (Prefabs.Primitive == null)
            throw new NullReferenceException(nameof(Prefabs.Primitive));

        if (!Prefabs.Primitive.TryGetComponent<PrimitiveObjectToy>(out PrimitiveObjectToy? primitiveToyBase))
            throw new NullReferenceException("PrimitiveObjectToy not found");

        Base = Object.Instantiate(primitiveToyBase);
        Base.SpawnerFootprint = new Footprint(Server.Host.ReferenceHub);
        NetworkServer.Spawn(Base.gameObject);

        Position = position;
        Rotation = rotation;
        Scale = size == default ? Vector3.one : size;

        Type = type;
        Color = color == default ? Color.white : color;
        Collider = collider;
        Visible = visible;

        Map.Primitives.Add(this);
        NonStaticPrims.Add(this);
    }

    public override bool IsStatic
    {
        get => base.IsStatic;
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

    public override void Destroy()
    {
        NetworkServer.Destroy(Base.gameObject);
        Map.Primitives.Remove(this);
        NonStaticPrims.Remove(this);
    }
}