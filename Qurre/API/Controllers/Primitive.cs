using AdminToys;
using UnityEngine;
using Mirror;
using System;

namespace Qurre.API.Controllers
{
    public class Primitive
    {
        public byte MovementSmoothing
        {
            get => Base.NetworkMovementSmoothing;
            set => Base.NetworkMovementSmoothing = value;
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

        private protected bool _collider = true;
        public bool Collider
        {
            get => _collider;
            set
            {
                _collider = value;
                Vector3 _s = Scale;
                if (_collider) Base.transform.localScale = new Vector3(Math.Abs(_s.x), Math.Abs(_s.y), Math.Abs(_s.z));
                else Base.transform.localScale = new Vector3(-Math.Abs(_s.x), -Math.Abs(_s.y), -Math.Abs(_s.z));
            }
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
        }

        public PrimitiveObjectToy Base { get; }

        public Primitive(PrimitiveType type) : this(type, Vector3.zero) { }
        public Primitive(PrimitiveType type, Vector3 position, Color color = default,
            Quaternion rotation = default, Vector3 size = default, bool collider = true)
        {
            try
            {
                if (!Addons.Prefabs.Primitive.TryGetComponent<AdminToyBase>(out var primitiveToyBase)) return;

                AdminToyBase prim = UnityEngine.Object.Instantiate(primitiveToyBase, position, rotation);

                Base = (PrimitiveObjectToy)prim;
                Base.SpawnerFootprint = new Footprinting.Footprint(Server.Host.ReferenceHub);
                NetworkServer.Spawn(Base.gameObject);

                Base.NetworkPrimitiveType = type;
                Base.NetworkMaterialColor = color == default ? Color.white : color;

                Base.transform.localPosition = position;
                Base.transform.localRotation = rotation;
                Base.transform.localScale = size == default ? Vector3.one : size;

                Base.NetworkScale = Base.transform.lossyScale;
                Base.NetworkPosition = Base.transform.position;
                Base.NetworkRotation = new LowPrecisionQuaternion(Base.transform.rotation);

                Collider = collider;
                Map.Primitives.Add(this);
            }
            catch (Exception e)
            {
                Log.Error($"{e}\n{e.StackTrace}");
            }
        }
    }
}