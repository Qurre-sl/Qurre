using UnityEngine;
using Interactables.Interobjects.DoorUtils;
using Interactables.Interobjects;
using Mirror;
using System.Linq;
using Qurre.API.Objects;
using Qurre.Internal.Misc;
using System.Collections.Generic;

namespace Qurre.API.Controllers
{
    public class Door
    {
        string name;
        DoorType type = DoorType.Unknown;
        List<Room> _rooms = new();

        public DoorVariant DoorVariant { get; internal set; }
        public GameObject GameObject => DoorVariant?.gameObject;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name)) return GameObject.name;
                return name;
            }
            set => name = value;
        }
        public List<Room> Rooms
        {
            get
            {
                if (_rooms.Count > 0)
                    return _rooms;

                _rooms = DoorVariant.Rooms.Select(x => x.GetRoom()).ToList();

                return _rooms;
            }
        }

        public Vector3 Position
        {
            get => GameObject.transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.position = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public Quaternion Rotation
        {
            get => GameObject.transform.rotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.rotation = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public Vector3 Scale
        {
            get => GameObject.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        public bool IsLift => DoorVariant is ElevatorDoor;
        public bool Breakable => DoorVariant is BreakableDoor;
        public bool Pryable
        {
            get
            {
                if (DoorVariant is PryableDoor pry)
                    return pry.TryPryGate(ReferenceHub.HostHub);
                else return false;
            }
        }
        public DoorPermissions Permissions
        {
            get => DoorVariant.RequiredPermissions;
            set => DoorVariant.RequiredPermissions = value;
        }
        public DoorType Type
        {
            get
            {
                if (type != DoorType.Unknown) return type;

                foreach (var _type in (System.Enum.GetValues(typeof(DoorType)) as DoorType[])
                    .Where(_type => _type.ToString().ToUpper().Contains(Name.ToUpper())))
                    return type = _type;

                // if else if else if else if else if else if ; yes, yes, i know, yandere my sensei
                if (Name.Contains("EZ BreakableDoor")) type = DoorType.EZ_Door;
                else if (Name.Contains("LCZ BreakableDoor")) type = DoorType.LCZ_Door;
                else if (Name.Contains("HCZ BreakableDoor")) type = DoorType.HCZ_Door;
                else if (Name.Contains("Prison BreakableDoor")) type = DoorType.PrisonDoor;
                else if (Name.Contains("LCZ PortallessBreakableDoor")) type = DoorType.Airlock;
                else if (Name.Contains("Unsecured Pryable GateDoor")) type = DoorType.HCZ_049_Gate;
                else type = DoorType.Unknown;

                return type;
            }
        }

        public bool Open
        {
            get => DoorVariant.IsConsideredOpen();
            set => DoorVariant.NetworkTargetState = value;
        }
        public bool Lock
        {
            get => DoorVariant.ActiveLocks > 0;
            set => DoorVariant.ServerChangeLock(DoorLockReason.SpecialDoorFeature, value);
        }
        public bool Destroyed
        {
            get
            {
                if (DoorVariant is BreakableDoor damageableDoor)
                    return damageableDoor.IsDestroyed;
                else return false;
            }
        }

        public bool Break()
        {
            if (DoorVariant is BreakableDoor damageableDoor)
            {
                damageableDoor.IsDestroyed = true;
                return true;
            }
            else return false;
        }
        public void Destroy()
        {
            NetworkServer.UnSpawn(GameObject);
            Map.Doors.Remove(this);
            Object.Destroy(GameObject);
        }

        internal Door(DoorVariant _)
        {
            DoorVariant = _;

            if (DoorVariant.TryGetComponent<DoorNametagExtension>(out var nametag))
                Name = nametag.GetName;
        }
        public Door(Vector3 position, DoorPrefabs prefab, Quaternion? rotation = null, DoorPermissions permissions = null)
        {
            DoorVariant = Object.Instantiate(prefab.GetPrefab());

            DoorVariant.transform.position = position;
            DoorVariant.transform.rotation = rotation ?? new();
            DoorVariant.RequiredPermissions = permissions ?? new();

            if (DoorVariant.TryGetComponent<DoorNametagExtension>(out var nametag))
                Name = nametag.GetName;

            NetworkServer.Spawn(DoorVariant.gameObject);

            DoorVariant.netIdentity.UpdateData();
            var comp = GameObject.AddComponent<DoorsUpdater>();
            if (comp) comp.Door = DoorVariant;

            Map.Doors.Add(this);
        }
    }
}