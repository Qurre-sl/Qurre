using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Mirror;
using Qurre.API.Objects;
using Qurre.Internal.Misc;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Door
    {
        string name;
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

                if (DoorVariant.Rooms is null)
                {
                    List<RoomIdentifier> _list = new();
                    Vector3 position = DoorVariant.transform.position;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3Int key = RoomIdUtils.PositionToCoords(position + DoorVariant.WorldDirections[i]);
                        if (RoomIdentifier.RoomsByCoordinates.TryGetValue(key, out var value) &&
                            CollectionExtensions.GetOrAdd(DoorVariant.DoorsByRoom, value, () => new HashSet<DoorVariant>()).Add(DoorVariant))
                        {
                            _list.Add(value);
                        }
                    }

                    _rooms = _list.Select(x => x.GetRoom()).ToList();
                    return _rooms;
                }

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

        public DoorType Type { get; private set; }
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

        void SetupDoorType()
        {
            Type = DoorType.Unknown;

            switch (Name)
            {
                case "LCZ_ARMORY":
                    Type = DoorType.LczArmory;
                    return;
                case "LCZ_CAFE":
                    Type = DoorType.LczCafe;
                    return;
                case "GR18_INNER":
                    Type = DoorType.LczGr18;
                    return;
                case "GR18":
                    Type = DoorType.LczGr18Gate;
                    return;
                case "LCZ_WC":
                    Type = DoorType.LczWc;
                    return;

                case "CHECKPOINT_LCZ_A":
                    Type = DoorType.LczCheckpointA;
                    return;
                case "CHECKPOINT_LCZ_B":
                    Type = DoorType.LczCheckpointB;
                    return;

                case "173_ARMORY":
                    Type = DoorType.Lcz173Armory;
                    return;
                case "173_GATE":
                    Type = DoorType.Lcz173Gate;
                    return;
                case "173_CONNECTOR":
                    Type = DoorType.Lcz173Connector;
                    return;
                case "173_BOTTOM":
                    Type = DoorType.Lcz173Bottom;
                    return;

                case "330":
                    Type = DoorType.Lcz330;
                    return;
                case "330_CHAMBER":
                    Type = DoorType.Lcz330Chamber;
                    return;

                case "914":
                    Type = DoorType.Lcz914Gate;
                    return;


                case "HCZ_ARMORY":
                    Type = DoorType.HczArmory;
                    return;

                case "HID":
                    Type = DoorType.HczHid;
                    return;
                case "HID_LEFT":
                    Type = DoorType.HczHidLeft;
                    return;
                case "HID_RIGHT":
                    Type = DoorType.HczHidRight;
                    return;

                case "NUKE_ARMORY":
                    Type = DoorType.HczNukeArmory;
                    return;
                case "SERVERS_BOTTOM":
                    Type = DoorType.HczServers;
                    return;

                case "049_ARMORY":
                    Type = DoorType.Hcz049Armory;
                    return;

                case "079_ARMORY":
                    Type = DoorType.Hcz079Armory;
                    return;
                case "079_FIRST":
                    Type = DoorType.Hcz079First;
                    return;
                case "079_SECOND":
                    Type = DoorType.Hcz079Second;
                    return;

                case "096":
                    Type = DoorType.Hcz096;
                    return;

                case "106_PRIMARY":
                    Type = DoorType.Hcz106First;
                    return;
                case "106_SECONDARY":
                    Type = DoorType.Hcz106Second;
                    return;

                case "939_CRYO":
                    Type = DoorType.Hcz939;
                    return;


                case "INTERCOM":
                    Type = DoorType.EzIntercom;
                    return;

                case "CHECKPOINT_EZ_HCZ_A":
                    Type = DoorType.EzCheckpointA;
                    return;
                case "CHECKPOINT_EZ_HCZ_B":
                    Type = DoorType.EzCheckpointB;
                    return;

                case "GATE_A":
                    Type = DoorType.EzGateA;
                    return;
                case "GATE_B":
                    Type = DoorType.EzGateB;
                    return;


                case "SURFACE_GATE":
                    Type = DoorType.SurfaceGate;
                    return;
                case "ESCAPE_PRIMARY":
                    Type = DoorType.SurfaceEscapeFirst;
                    return;
                case "ESCAPE_SECONDARY":
                    Type = DoorType.SurfaceEscapeSecond;
                    return;
                case "SURFACE_NUKE":
                    Type = DoorType.SurfaceNuke;
                    return;
            }

            switch (Name.Split(' ')[0])
            {
                case "LCZ":
                    {
                        if (Name.StartsWith("LCZ PortallessBreakableDoor"))
                        {
                            Type = DoorType.LczAirlock;
                            return;
                        }

                        Type = DoorType.LczStandart;
                        return;
                    }
                case "HCZ":
                    Type = DoorType.HczStandart;
                    return;
                case "EZ":
                    Type = DoorType.EzStandart;
                    return;

                case "Prison":
                    Type = DoorType.LczPrison;
                    return;

                case "914":
                    Type = DoorType.Lcz914Chamber;
                    return;

                case "Unsecured":
                    {
                        if (DoorVariant.Rooms.Any(x => x.Name == RoomName.Hcz049))
                        {
                            if (Name.Contains("(1)"))
                                Type = DoorType.Hcz173Gate;
                            else
                                Type = DoorType.Hcz049Gate;
                            return;
                        }

                        if (DoorVariant.Rooms.Any(x => x.Name == RoomName.HczCheckpointToEntranceZone))
                        {
                            Type = DoorType.EzCheckpointGate;
                            return;
                        }

                        Type = DoorType.Unknown;
                        return;
                    }

                case "Intercom":
                    {
                        foreach (var room in DoorVariant.Rooms)
                        {
                            switch (room.Name)
                            {
                                case RoomName.HczCheckpointA:
                                    {
                                        Type = DoorType.EzCheckpointArmoryA;
                                        return;
                                    }
                                case RoomName.HczCheckpointB:
                                    {
                                        Type = DoorType.EzCheckpointArmoryB;
                                        return;
                                    }
                                default:
                                    {
                                        if (room.gameObject.name.StartsWith("HCZ Part"))
                                        {
                                            switch (room.gameObject.transform.parent.name)
                                            {
                                                case "HCZ_EZ_Checkpoint (A)":
                                                    Type = DoorType.EzCheckpointArmoryA;
                                                    return;
                                                case "HCZ_EZ_Checkpoint (B)":
                                                    Type = DoorType.EzCheckpointArmoryB;
                                                    return;
                                            }
                                        }
                                        break;
                                    }
                            }
                        }

                        Type = DoorType.Unknown;
                        return;
                    }

                case "Elevator":
                    {
                        if (!DoorVariant.Rooms.Any())
                            return;

                        switch (DoorVariant.Rooms[0].Name)
                        {
                            case RoomName.LczCheckpointA:
                                Type = DoorType.ElevatorLczChkpA;
                                return;
                            case RoomName.LczCheckpointB:
                                Type = DoorType.ElevatorLczChkpB;
                                return;

                            case RoomName.HczCheckpointA:
                                Type = DoorType.ElevatorHczChkpA;
                                return;
                            case RoomName.HczCheckpointB:
                                Type = DoorType.ElevatorHczChkpB;
                                return;

                            case RoomName.Hcz049:
                                Type = DoorType.Elevator049;
                                return;

                            case RoomName.HczWarhead:
                                Type = DoorType.ElevatorNuke;
                                return;

                            case RoomName.EzGateA:
                                Type = DoorType.ElevatorGateA;
                                return;
                            case RoomName.EzGateB:
                                Type = DoorType.ElevatorGateB;
                                return;
                            case RoomName.Outside:
                                {
                                    if (DoorVariant is not ElevatorDoor elev)
                                        return;

                                    switch (elev.Group)
                                    {
                                        case ElevatorManager.ElevatorGroup.GateA:
                                            Type = DoorType.EzGateA;
                                            return;
                                        case ElevatorManager.ElevatorGroup.GateB:
                                            Type = DoorType.EzGateB;
                                            return;
                                    }

                                    return;
                                }
                        }

                        return;
                    }
            }
        }

        internal Door(DoorVariant _)
        {
            DoorVariant = _;

            if (DoorVariant.TryGetComponent<DoorNametagExtension>(out var nametag))
                Name = nametag.GetName;

            SetupDoorType();
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

            SetupDoorType();
        }
    }
}