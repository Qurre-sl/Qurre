using System.Linq;
using JetBrains.Annotations;
using MapGeneration.Distributors;
using Mirror;
using Qurre.API.Controllers.Structs;
using Qurre.API.Objects;
using Qurre.Internal.Misc;
using UnityEngine;

namespace Qurre.API.Controllers;

using BaseLocker = MapGeneration.Distributors.Locker;

[PublicAPI]
public class Locker
{
    private LockerType _typeCached = LockerType.Unknown;

    internal Locker(BaseLocker locker)
    {
        Custom = false;
        GlobalLocker = locker;
        Chambers = GlobalLocker.Chambers.Select(x => new Chamber(x, this)).ToArray();
    }

    public Locker(Vector3 position, LockerPrefabs type, Quaternion? rotation = null)
    {
        Custom = true;
        GlobalLocker = Object.Instantiate(type.GetPrefab());

        GlobalLocker.transform.position = position;
        GlobalLocker.transform.rotation = rotation ?? new Quaternion();

        Chambers = GlobalLocker.Chambers.Select(x => new Chamber(x, this)).ToArray();

        NetworkServer.Spawn(GlobalLocker.gameObject);
        GlobalLocker.netIdentity.UpdateData();

        LockersUpdater? comp = GameObject.AddComponent<LockersUpdater>();
        if (comp)
        {
            comp.Locker = GlobalLocker;
            comp.Init();
        }

        Map.Lockers.Add(this);
    }

    public GameObject GameObject => GlobalLocker.gameObject;
    public Transform Transform => GlobalLocker.transform;
    public LockerLoot[] Loot => GlobalLocker.Loot;
    public AudioClip GrantedBeep => GlobalLocker._grantedBeep;
    public AudioClip DeniedBeep => GlobalLocker._deniedBeep;
    public string Name => GlobalLocker.name;

    public BaseLocker GlobalLocker { get; }
    public Chamber[] Chambers { get; private set; }
    public bool Custom { get; private set; }

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
            if (_typeCached is LockerType.Unknown) _typeCached = Get();
            return _typeCached;

            LockerType Get()
            {
                if (Name.Contains("AdrenalineMedkit")) return LockerType.AdrenalineMedkit;
                if (Name.Contains("RegularMedkit")) return LockerType.RegularMedkit;
                if (Name.Contains("Pedestal")) return LockerType.Pedestal;
                if (Name.Contains("MiscLocker")) return LockerType.MiscLocker;
                if (Name.Contains("RifleRack")) return LockerType.RifleRack;
                if (Name.Contains("LargeGunLocker")) return LockerType.LargeGun;
                return LockerType.Unknown;
            } // end void 'Get'
        } // end Type_get
    } // end field
}