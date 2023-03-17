using MapGeneration.Distributors;
using BaseLocker = MapGeneration.Distributors.Locker;
using Qurre.API.Objects;
using UnityEngine;
using Mirror;
using Qurre.API.Controllers.Structs;
using System.Linq;
using Qurre.Internal.Misc;

namespace Qurre.API.Controllers
{
    public class Locker
    {
        internal readonly BaseLocker _locker;
        private LockerType _typeCached = LockerType.Unknown;

        public GameObject GameObject => _locker.gameObject;
        public Transform Transform => _locker.transform;
        public BaseLocker GlobalLocker => _locker;
        public LockerLoot[] Loot => _locker.Loot;
        public AudioClip GrantedBeep => _locker._grantedBeep;
        public AudioClip DeniedBeep => _locker._deniedBeep;
        public Chamber[] Chambers { get; private set; }

        public string Name => _locker.name;

        public Vector3 Position
        {
            get => Transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                Transform.position = value;
                NetworkServer.Spawn(GameObject);
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

        public LockerType Type
        {
            get
            {
                if (_typeCached is LockerType.Unknown) _typeCached = _get();
                return _typeCached;
                LockerType _get()
                {
                    if (Name.Contains("AdrenalineMedkit")) return LockerType.AdrenalineMedkit;
                    if (Name.Contains("RegularMedkit")) return LockerType.RegularMedkit;
                    if (Name.Contains("Pedestal")) return LockerType.Pedestal;
                    if (Name.Contains("MiscLocker")) return LockerType.MiscLocker;
                    if (Name.Contains("RifleRack")) return LockerType.RifleRack;
                    if (Name.Contains("LargeGunLocker")) return LockerType.LargeGun;
                    return LockerType.Unknown;
                };
            }
        }

        internal Locker(BaseLocker locker)
        {
            _locker = locker;
            Chambers = _locker.Chambers.Select(x => new Chamber(x, this)).ToArray();
        }
        public Locker(Vector3 position, LockerPrefabs type, Quaternion? rotation = null)
        {
            _locker = Object.Instantiate(type.GetPrefab());

            _locker.transform.position = position;
            _locker.transform.rotation = rotation ?? new();

            Chambers = _locker.Chambers.Select(x => new Chamber(x, this)).ToArray();

            NetworkServer.Spawn(_locker.gameObject);
            _locker.netIdentity.UpdateData();
            var comp = GameObject.AddComponent<LockersUpdater>();
            if (comp) comp.Locker = _locker;

            Map.Lockers.Add(this);
        }
    }
}