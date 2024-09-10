using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum RoomType : byte
{
    LczClassDSpawn,
    LczThreeWay,
    LczCrossing,
    LczCurve,
    LczStraight,
    LczAirlock,
    LczArmory,
    LczCafe,
    LczToilets,
    LczPlants,
    LczChkpA,
    LczChkpB,
    Lcz173,
    Lcz330,
    Lcz914,
    LczGr18,

    HczThreeWay,
    HczCrossing,
    HczCurve,
    HczStraight,
    HczTesla,
    HczArmory,
    HczHid,
    HczNuke,
    HczServers,
    HczChkpA,
    HczChkpB,
    Hcz049,
    Hcz079,
    Hcz096,
    Hcz106,
    Hcz939,
    HczPart,
    HczTestroom,

    EzThreeWay,
    EzCrossing,
    EzCurve,
    EzStraight,
    EzCafeteria,
    EzUpstairsPcs,
    EzDownstairsPcs,
    EzPcs,
    EzSmall,
    EzIntercom,
    EzShelter,
    EzVent,
    EzGateA,
    EzGateB,
    EzPart,
    EzCollapsedTunnel,

    Surface,
    Pocket,

    Unknown = 0
}