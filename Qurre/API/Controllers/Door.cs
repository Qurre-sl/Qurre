using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using JetBrains.Annotations;
using MapGeneration;
using Mirror;
using Qurre.API.Controllers.Components;
using Qurre.API.Objects;
using Qurre.API.World;
using Qurre.Internal.Misc;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Door : NetTransform
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

    public Door(Vector3 position, DoorPrefabs prefab, Quaternion? rotation = null,
        DoorPermissionsPolicy? permissions = null)
    {
        Custom = true;
        PrefabType = prefab;

        DoorVariant = Object.Instantiate(prefab.GetPrefab());
        _cachedGameObject = DoorVariant.gameObject;

        DoorVariant.transform.position = position;
        DoorVariant.transform.rotation = rotation ?? new Quaternion();
        DoorVariant.RequiredPermissions = permissions ?? new DoorPermissionsPolicy();

        if (DoorVariant.TryGetComponent<DoorNametagExtension>(out DoorNametagExtension? nameTag))
            Name = nameTag.GetName;

        NetworkServer.Spawn(DoorVariant.gameObject);

        DoorVariant.netIdentity.UpdateData();
        DoorsUpdater? comp = DoorVariant.gameObject.AddComponent<DoorsUpdater>();
        if (comp)
        {
            comp.Door = DoorVariant;
            comp.Init();
        }

        Map.Doors.Add(this);

        SetupDoorType();
    }

    public bool Custom { get; init; }
    public DoorPrefabs PrefabType { get; init; }

    public DoorType Type { get; private set; }
    public DoorVariant DoorVariant { get; internal set; }

    public override GameObject GameObject => DoorVariant != null ? DoorVariant.gameObject : _cachedGameObject;

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

    public bool Pryable
    {
        get
        {
            if (DoorVariant is not PryableDoor pry)
                return false;

            if (!ReferenceHub.TryGetHostHub(out ReferenceHub? hub))
                return false;

            return pry.TryPryGate(hub);
        }
    }

    public DoorPermissionsPolicy Permissions
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

    public override void Destroy()
    {
        NetworkServer.Destroy(GameObject);
        Map.Doors.Remove(this);
    }

    private void SetupDoorType()
    {
        Type = DoorType.Unknown;

#if TESTS
        Log.Custom($"Name: {Name};", "TESTS: API.Door");
#endif

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

            case "HID_CHAMBER":
                Type = DoorType.HczHid;
                return;
            case "HID_LOWER":
                Type = DoorType.HczHidLower;
                return;
            case "HID_UPPER":
                Type = DoorType.HczHidUpper;
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
                {
                    foreach (RoomIdentifier? room in DoorVariant.Rooms)
                        if (room.Name == RoomName.HczCheckpointToEntranceZone)
                            switch (room.transform.position.z)
                            {
                                case > 75:
                                    {
                                        Type = DoorType.EzCheckpointA;
                                        return;
                                    }
                                default:
                                    {
                                        Type = DoorType.EzCheckpointB;
                                        return;
                                    }
                            }

                    return;
                }

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
            case "ESCAPE_FINAL":
                Type = DoorType.SurfaceEscapeFinal;
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
                if (Name.StartsWith("HCZ BulkDoor"))
                {
                    Type = DoorType.HczBulk;
                    return;
                }

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
                        if (room.Name == RoomName.HczCheckpointToEntranceZone)
                            switch (room.transform.position.z)
                            {
                                case > 75:
                                    {
                                        Type = DoorType.EzCheckpointArmoryA;
                                        return;
                                    }
                                default:
                                    {
                                        Type = DoorType.EzCheckpointArmoryB;
                                        return;
                                    }
                            }

                    Type = DoorType.Unknown;
                    return;
                }

            case "Elevator" or "Nuke":
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
                                    case ElevatorGroup.GateA:
                                        Type = DoorType.EzGateA;
                                        return;
                                    case ElevatorGroup.GateB:
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