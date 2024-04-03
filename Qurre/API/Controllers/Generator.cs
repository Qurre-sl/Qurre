using MapGeneration.Distributors;
using Mirror;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Generator
    {
        private string name;
        private readonly Scp079Generator generator;
        private readonly StructurePositionSync positionsync;

        public GameObject GameObject => generator.gameObject;
        public Transform Transform => GameObject.transform;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name)) return GameObject.name;
                return name;
            }
            set => name = value;
        }

        public Vector3 Position
        {
            get => Transform.position;
            set
            {
                positionsync.Network_position = value;
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
                positionsync.Network_rotationY = (sbyte)(value.eulerAngles.y / 5.625f);
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
            get => generator.HasFlag(generator._flags, Scp079Generator.GeneratorFlags.Open);
            set
            {
                generator.ServerSetFlag(Scp079Generator.GeneratorFlags.Open, value);
                generator._targetCooldown = generator._doorToggleCooldownTime;
            }
        }
        public bool Lock
        {
            get => !generator.HasFlag(generator._flags, Scp079Generator.GeneratorFlags.Unlocked);
            set
            {
                generator.ServerSetFlag(Scp079Generator.GeneratorFlags.Unlocked, !value);
                generator._targetCooldown = generator._unlockCooldownTime;
            }
        }
        public bool Active
        {
            get => generator.Activating;
            set
            {
                generator.Activating = value;
                if (value) generator._leverStopwatch.Restart();
                generator._targetCooldown = generator._doorToggleCooldownTime;
            }
        }
        public bool Engaged
        {
            get => generator.Engaged;
            set => generator.Engaged = value;
        }
        public short Time
        {
            get => generator._syncTime;
            set => generator.Network_syncTime = value;
        }

        public void Destroy()
        {
            Map.Generators.Remove(this);
            NetworkServer.Destroy(GameObject);
        }

        internal Generator(Scp079Generator g)
        {
            generator = g;
            positionsync = generator.GetComponent<StructurePositionSync>();
        }
        public Generator(Vector3 position, Quaternion? rotation = null)
        {
            generator = Object.Instantiate(Addons.Prefabs.Generator);

            generator.transform.position = position;
            generator.transform.rotation = rotation ?? new();

            positionsync = generator.GetComponent<StructurePositionSync>();

            NetworkServer.Spawn(generator.gameObject);

            generator.netIdentity.UpdateData();

            Map.Generators.Add(this);
        }
    }
}