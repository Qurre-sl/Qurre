using AdminToys;
using Mirror;
using Qurre.API.Objects;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class ShootingTarget
    {
        public Vector3 Position
        {
            get => Base.transform.position;
            set
            {
                NetworkServer.UnSpawn(Base.gameObject);
                Base.transform.position = value;
                NetworkServer.Spawn(Base.gameObject);
            }
        }
        public Vector3 Scale
        {
            get => Base.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(Base.gameObject);
                Base.transform.localScale = value;
                NetworkServer.Spawn(Base.gameObject);
            }
        }
        public Quaternion Rotation
        {
            get => Base.transform.localRotation;
            set
            {
                NetworkServer.UnSpawn(Base.gameObject);
                Base.transform.localRotation = value;
                NetworkServer.Spawn(Base.gameObject);
            }
        }

        public TargetPrefabs Type { get; }

        public void Clear() => Base.ClearTarget();
        public void Destroy()
        {
            NetworkServer.Destroy(Base.gameObject);
            Map.ShootingTargets.Remove(this);
        }

        public AdminToys.ShootingTarget Base { get; }

        public ShootingTarget(TargetPrefabs type, Vector3 position, Quaternion rotation = default, Vector3 size = default)
        {
            if (!type.GetPrefab().TryGetComponent<AdminToyBase>(out var primitiveToyBase)) return;

            var prim = Object.Instantiate(primitiveToyBase, position, rotation);

            Type = type;
            Base = (AdminToys.ShootingTarget)prim;
            Base.transform.localScale = size == default ? Vector3.one : size;
            NetworkServer.Spawn(Base.gameObject);

            Map.ShootingTargets.Add(this);
        }
    }
}