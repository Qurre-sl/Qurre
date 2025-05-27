using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using Qurre.API.Objects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Room
{
    internal static readonly List<NetworkIdentity> NetworkIdentities = [];

    internal readonly Color DefaultColor;
    internal readonly RoomLightController[] GameLights;

    internal Room(RoomIdentifier identifier)
    {
        Identifier = identifier;
        RoomName = Identifier.Name;
        Shape = Identifier.Shape;
        GameObject = identifier.gameObject;

        GameLights = GameObject.GetComponentsInChildren<RoomLightController>();

        DefaultColor = GameLights.Length == 0 ? Color.white : GameLights[0].OverrideColor;

        foreach (Scp079Camera? cam in GameObject.GetComponentsInChildren<Scp079Camera>())
            Cameras.Add(new Camera(cam, this));

        Lights = new Lights(this);

        Type = GetType(Name, Transform);

        NetworkIdentity = GetNetworkIdentity();
    }

    public Lights Lights { get; private set; }
    public GameObject GameObject { get; }
    public RoomIdentifier Identifier { get; }
    public NetworkIdentity? NetworkIdentity { get; }

    public List<Door> Doors { get; } = [];
    public List<Camera> Cameras { get; } = [];

    public RoomName RoomName { get; }
    public RoomShape Shape { get; }
    public RoomType Type { get; }

    public Tesla? Tesla => GameObject.GetComponentInChildren<TeslaGate>()?.GetTesla();
    public Transform Transform => GameObject.transform;
    public string Name => GameObject.name;

    public IReadOnlyCollection<Player> Players =>
        [.. Player.List.Where(x => !x.IsHost && x.GamePlay.Room.Name == Name)];

    public bool LightsDisabled => GameLights.Length > 0 && GameLights.Any(x => !x.NetworkLightsEnabled);

    public Vector3 Position
    {
        get => Transform.position;
        set
        {
            if (NetworkIdentity == null)
                return;

            NetworkServer.UnSpawn(NetworkIdentity.gameObject);
            NetworkIdentity.transform.position = value;
            NetworkServer.Spawn(NetworkIdentity.gameObject);
            NetworkIdentity.UpdateData();
        }
    }

    public Quaternion Rotation
    {
        get => Transform.rotation;
        set
        {
            if (NetworkIdentity == null)
                return;

            NetworkIdentity.transform.rotation = value;
            NetworkIdentity.UpdateData();
        }
    }

    public Vector3 Scale
    {
        get => Transform.localScale;
        set
        {
            if (NetworkIdentity == null)
                return;

            NetworkIdentity.transform.localScale = value;
            NetworkIdentity.UpdateData();
        }
    }

    public FacilityZone Zone => LabApi.Features.Wrappers.Room.Get(Identifier).Zone;

    public void LightsOff(float duration)
    {
        foreach (RoomLightController light in GameLights)
            light.ServerFlickerLights(duration);
    }

    private NetworkIdentity? GetNetworkIdentity()
    {
        if (NetworkIdentities.Count == 0)
            NetworkIdentities.AddRange(Object.FindObjectsOfType<NetworkIdentity>().Where(x => x.name.Contains("All")));

        if (NetworkIdentities.TryFind(out var ident, x => Vector3.Distance(x.transform.position, Position) < 0.1f))
            return ident;

        return null;
    }

    private static RoomType GetType(string name, Transform trans)
    {
        string rawName = name;
        int arr = rawName.IndexOf('(');
        if (arr > 0) rawName = rawName.Remove(arr, rawName.Length - arr).Trim();

#if TESTS
        Log.Custom($"rawName: {rawName};", "TESTS: API.Room");
        if (rawName.EndsWith(" Part"))
            Log.Custom($"ZZZ POSITION::::: {Addons.BetterColors.Red(trans.position.z)}");
#endif

        return rawName switch
        {
            "LCZ_ClassDSpawn" => RoomType.LczClassDSpawn,
            "LCZ_TCross" => RoomType.LczThreeWay,
            "LCZ_Crossing" => RoomType.LczCrossing,
            "LCZ_Curve" => RoomType.LczCurve,
            "LCZ_Straight" => RoomType.LczStraight,
            "LCZ_Airlock" => RoomType.LczAirlock,
            "LCZ_Armory" => RoomType.LczArmory,
            "LCZ_Cafe" => RoomType.LczCafe,
            "LCZ_Toilets" => RoomType.LczToilets,
            "LCZ_Plants" => RoomType.LczPlants,
            "LCZ_ChkpA" => RoomType.LczChkpA,
            "LCZ_ChkpB" => RoomType.LczChkpB,
            "LCZ_173" => RoomType.Lcz173,
            "LCZ_330" => RoomType.Lcz330,
            "LCZ_914" => RoomType.Lcz914,
            "LCZ_372" => RoomType.LczGr18,

            "HCZ_Crossing" => RoomType.HczCrossing,
            "HCZ_Curve" => RoomType.HczCurve,
            "HCZ_Straight" or "HCZ_Straight Variant" => RoomType.HczStraight,
            "HCZ_Intersection" => RoomType.HczIntersection,
            "HCZ_Corner_Deep" => RoomType.HczCornerDeep,
            "HCZ_Intersection_Junk" => RoomType.HczJunk,
            "HCZ_Straight_PipeRoom" => RoomType.HczPipe,
            "HCZ_Straight_C" => RoomType.HczToilets,
            "HCZ_Tesla_Rework" => RoomType.HczTesla,
            "HCZ_TArmory" => RoomType.HczArmory,
            "HCZ_MicroHID_New" => RoomType.HczHid,
            "HCZ_Nuke" => RoomType.HczNuke,
            "HCZ_ChkpA" => RoomType.HczChkpA,
            "HCZ_ChkpB" => RoomType.HczChkpB,
            "HCZ_049" => RoomType.Hcz049,
            "HCZ_079" => RoomType.Hcz079,
            "HCZ_096" => RoomType.Hcz096,
            "HCZ_106_Rework" => RoomType.Hcz106,
            "HCZ_939" => RoomType.Hcz939,
            "HCZ_Testroom" => RoomType.HczTest,
            "HCZ_Crossroom_Water" => RoomType.HczWater,

            "EZ_ThreeWay" => RoomType.EzThreeWay,
            "EZ_Crossing" => RoomType.EzCrossing,
            "EZ_Curve" => RoomType.EzCurve,
            "EZ_Straight" or
                "EZ_Cafeteria" or
                "EZ_StraightColumn" => RoomType.EzStraight,
            "EZ_upstairs" => RoomType.EzUpstairsPcs,
            "EZ_PCs_small" => RoomType.EzDownstairsPcs,
            "EZ_PCs" => RoomType.EzPcs,
            "EZ_Smallrooms1" => RoomType.EzSmall1,
            "EZ_Smallrooms2" => RoomType.EzSmall2,
            "EZ_Chef" => RoomType.EzChef,
            "EZ_Intercom" => RoomType.EzIntercom,
            "EZ_Shelter" => RoomType.EzShelter,
            "EZ_Endoof" => RoomType.EzVent,
            "EZ_GateA" => RoomType.EzGateA,
            "EZ_GateB" => RoomType.EzGateB,
            "HCZ_EZ_Checkpoint Part" => trans.position.z switch
            {
                // 60...90
                > 75 => RoomType.HczChkpA,
                _ => RoomType.HczChkpB
            },
            "EZ_HCZ_Checkpoint Part" => trans.position.z switch
            {
                // 60...90
                > 75 => RoomType.EzChkpA,
                _ => RoomType.EzChkpB
            },
            "EZ_CollapsedTunnel" => RoomType.EzCollapsedTunnel,

            "Outside" => RoomType.Surface,
            "PocketWorld" => RoomType.Pocket,

            _ => RoomType.Unknown
        };
    }
}