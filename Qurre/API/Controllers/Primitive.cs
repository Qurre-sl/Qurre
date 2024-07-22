using AdminToys;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Primitive
    {
        static internal bool _allowStatic = false;
        static internal readonly HashSet<Primitive> _cachedToSetStatic = new();
        static internal readonly HashSet<Primitive> _nonstaticPrims = new();

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

                if (_allowStatic)
                {
                    Base.NetworkIsStatic = value;

                    if (value)
                        _nonstaticPrims.Remove(this);
                    else
                        _nonstaticPrims.Add(this);

                    if (!value && _cachedToSetStatic.Contains(this))
                        _cachedToSetStatic.Remove(this);
                    return;
                }

                if (value)
                    _cachedToSetStatic.Add(this);
                else
                    _cachedToSetStatic.Remove(this);
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
        public Vector3 GlobalScale => Base.transform.lossyScale;

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
            _nonstaticPrims.Remove(this);
        }

        public PrimitiveObjectToy Base { get; }

        public Primitive(PrimitiveType type) : this(type, Vector3.zero) { }
        public Primitive(PrimitiveType type, Vector3 position, Color color = default,
            Quaternion rotation = default, Vector3 size = default, bool collider = true)
        {
            try
            {
                if (!Addons.Prefabs.Primitive.TryGetComponent<AdminToyBase>(out var primitiveToyBase))
                    return;

                AdminToyBase prim = UnityEngine.Object.Instantiate(primitiveToyBase, position, rotation);

                Base = (PrimitiveObjectToy)prim;
                Base.SpawnerFootprint = new Footprinting.Footprint(Server.Host.ReferenceHub);
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
                _nonstaticPrims.Add(this);
            }
            catch (Exception e)
            {
                Log.Error($"{e}\n{e.StackTrace}");
            }
        }
    }
}