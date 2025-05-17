using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum RoomType : byte
{
    Unknown,

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

    HczCrossing,
    HczCurve,
    HczStraight,
    HczIntersection,
    HczCornerDeep,
    HczJunk,
    HczPipe,
    HczToilets,
    HczTesla,
    HczArmory,
    HczHid,
    HczNuke,
    Hcz049,
    Hcz079,
    Hcz096,
    Hcz106,
    Hcz939,
    HczTest,
    HczWater,

    HczChkpA,
    HczChkpB,

    EzChkpA,
    EzChkpB,

    EzThreeWay,
    EzCrossing,
    EzCurve,
    EzStraight,
    EzUpstairsPcs,
    EzDownstairsPcs,
    EzPcs,
    EzSmall1,
    EzSmall2,
    EzChef,
    EzIntercom,
    EzShelter,
    EzVent,
    EzGateA,
    EzGateB,
    EzCollapsedTunnel,

    Surface,
    Pocket
}