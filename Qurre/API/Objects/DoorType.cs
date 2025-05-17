using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum DoorType : byte
{
    Unknown,

    LczStandard, // LCZ BreakableDoor
    HczStandard, // HCZ BreakableDoor
    HczBulk, // HCZ BulkDoor
    EzStandard, // EZ BreakableDoor


    LczAirlock, // LCZ PortallessBreakableDoor (n)
    LczArmory, // LCZ_ARMORY
    LczCafe, // LCZ_CAFE
    LczGr18, // GR18_INNER
    LczGr18Gate, // GR18
    LczPrison, // Prison BreakableDoor (n)
    LczWc, // LCZ_WC

    LczCheckpointA, // CHECKPOINT_LCZ_A
    LczCheckpointB, // CHECKPOINT_LCZ_B

    Lcz173Armory, // 173_ARMORY
    Lcz173Bottom, // 173_BOTTOM
    Lcz173Connector, // 173_CONNECTOR
    Lcz173Gate, // 173_GATE

    Lcz330, // 330
    Lcz330Chamber, // 330_CHAMBER

    Lcz914Chamber, // 914 Door // 914 Door (1)
    Lcz914Gate, // 914


    HczArmory, // HCZ_ARMORY

    HczHid, // HID_CHAMBER
    HczHidLower, // HID_LOWER
    HczHidUpper, // HID_UPPER

    Hcz049Armory, // 049_ARMORY
    Hcz049Gate, // Unsecured...

    Hcz079Armory, // 079_ARMORY
    Hcz079First, // 079_FIRST
    Hcz079Second, // 079_SECOND

    Hcz096, // 096

    Hcz106First, // 106_PRIMARY
    Hcz106Second, // 106_SECONDARY

    Hcz173Gate, // Unsecured...

    Hcz939, // 939_CRYO


    EzIntercom, // INTERCOM

    EzCheckpointA, // CHECKPOINT_EZ_HCZ_A
    EzCheckpointB, // CHECKPOINT_EZ_HCZ_B
    EzCheckpointArmoryA, // Intercom BreakableDoor (n)
    EzCheckpointArmoryB, // Intercom BreakableDoor (n)
    EzCheckpointGate, // Unsecured...

    EzGateA, // GATE_A
    EzGateB, // GATE_B


    SurfaceGate, // SURFACE_GATE
    SurfaceEscapeFirst, // ESCAPE_PRIMARY
    SurfaceEscapeSecond, // ESCAPE_SECONDARY
    SurfaceEscapeFinal, // ESCAPE_FINAL
    SurfaceNuke, // SURFACE_NUKE

    ElevatorLczChkpA, // RoomName: LczCheckpointA
    ElevatorLczChkpB, // RoomName: LczCheckpointB
    ElevatorHczChkpA, // RoomName: HczCheckpointA
    ElevatorHczChkpB, // RoomName: HczCheckpointB
    Elevator049, // RoomName: Hcz049
    ElevatorNuke, // RoomName: HczWarhead
    ElevatorGateA, // RoomName: EzGateA / Outside & ElevatorGroup: GateA
    ElevatorGateB // RoomName: EzGateB / Outside & ElevatorGroup: GateB
}