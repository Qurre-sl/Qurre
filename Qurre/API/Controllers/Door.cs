using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using JetBrains.Annotations;
using MapGeneration;
using Mirror;
using Qurre.API.Objects;
using Qurre.Internal.Misc;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Door
{
    private GameObject _cachedGameObject;
    private string _name = string.Empty;
    private List<Room> _rooms = [];

    internal Door(DoorVariant doorVariant)
    {
        Custom = false;
        DoorVariant = doorVariant;
        _cachedGameObject = DoorVariant.gameObject;

        if (DoorVariant.TryGetComponent<DoorNametagExtension>(out DoorNametagExtension? nameTag))
            Name = nameTag.GetName;

        SetupDoorType();
    }

    public Door(Vector3 position, DoorPrefabs prefab, Quaternion? rotation = null, DoorPermissions? permissions = null)
    {
        Custom = true;
        DoorVariant = Object.Instantiate(prefab.GetPrefab());
        _cachedGameObject = DoorVariant.gameObject;

        DoorVariant.transform.position = position;
        DoorVariant.transform.rotation = rotation ?? new Quaternion();
        DoorVariant.RequiredPermissions = permissions ?? new DoorPermissions();

        if (DoorVariant.TryGetComponent<DoorNametagExtension>(out DoorNametagExtension? nameTag))
            Name = nameTag.GetName;

        NetworkServer.Spawn(DoorVariant.gameObject);

        DoorVariant.netIdentity.UpdateData();
        DoorsUpdater? comp = GameObject.AddComponent<DoorsUpdater>();
        if (comp)
        {
            comp.Door = DoorVariant;
            comp.Init();
        }

        Map.Doors.Add(this);

        SetupDoorType();
    }

    public bool Custom { get; private set; }
    public DoorType Type { get; private set; }

    public DoorVariant DoorVariant { get; internal set; }
    public GameObject GameObject => DoorVariant != null ? DoorVariant.gameObject : _cachedGameObject;
    public bool IsLift => DoorVariant is ElevatorDoor;
    public bool Breakable => DoorVariant is BreakableDoor;

    public string Name
    {
        get => string.IsNullOrEmpty(_name) ? GameObject.name : _name;
        set => _name = value;
    }

    public List<Room> Rooms
    {
        get
        {
            if (_rooms.Count != 0)
                return _rooms;

            if (DoorVariant == null || DoorVariant.Rooms is null)
                throw new Exception("DoorVariant.Rooms is null");

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

    public bool Pryable
    {
        get
        {
            if (DoorVariant is PryableDoor pry)
                return pry.TryPryGate(ReferenceHub.HostHub);
            return false;
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
            return false;
        }
    }

    public bool Break()
    {
        if (DoorVariant is not BreakableDoor damageableDoor)
            return false;

        damageableDoor.IsDestroyed = true;
        return true;
    }

    public void Destroy()
    {
        Map.Doors.Remove(this);
        NetworkServer.Destroy(GameObject);
    }

    private void SetupDoorType()
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

                Type = DoorType.LczStandard;
                return;
            }
            case "HCZ":
                Type = DoorType.HczStandard;
                return;
            case "EZ":
                Type = DoorType.EzStandard;
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
                    Type = Name.Contains("(1)") ? DoorType.Hcz173Gate : DoorType.Hcz049Gate;
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
                foreach (RoomIdentifier? room in DoorVariant.Rooms)
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
                                switch (room.gameObject.transform.parent.name)
                                {
                                    case "HCZ_EZ_Checkpoint (A)":
                                        Type = DoorType.EzCheckpointArmoryA;
                                        return;
                                    case "HCZ_EZ_Checkpoint (B)":
                                        Type = DoorType.EzCheckpointArmoryB;
                                        return;
                                }

                            break;
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
                } // end switch(Room.Name)

                return;
            } // end case
        } // end switch(Name)
    } // end void
}