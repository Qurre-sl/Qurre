using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using Qurre.API.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Room
    {
        internal readonly Color defaultColor;
        internal readonly RoomLightController[] _lights;
        ZoneType zone = ZoneType.Unknown;

#nullable enable
        public Lights Lights { get; private set; }
        public GameObject GameObject { get; }
        public RoomIdentifier Identifier { get; }
        public NetworkIdentity? NetworkIdentity { get; }
        public Tesla? Tesla => GameObject.GetComponentInChildren<TeslaGate>()?.GetTesla();
        public Transform Transform => GameObject.transform;
#nullable restore

        public string Name => GameObject.name;
        public List<Door> Doors { get; } = new();
        public List<Camera> Cameras { get; } = new();
        public List<Player> Players => Player.List.Where(x => !x.IsHost && x.GamePlay.Room.Name == Name).ToList();

        public Vector3 Position
        {
            get => Transform.position;
            set
            {
                if (NetworkIdentity is null) return;
                NetworkIdentity.transform.position = value;
                NetworkIdentity.UpdateData();
            }
        }
        public Quaternion Rotation
        {
            get => Transform.rotation;
            set
            {
                if (NetworkIdentity is null) return;
                NetworkIdentity.transform.rotation = value;
                NetworkIdentity.UpdateData();
            }
        }
        public Vector3 Scale
        {
            get => Transform.localScale;
            set
            {
                if (NetworkIdentity is null) return;
                NetworkIdentity.transform.localScale = value;
                NetworkIdentity.UpdateData();
            }
        }

        public RoomName RoomName { get; }
        public RoomShape Shape { get; }
        public RoomType Type { get; }

        public ZoneType Zone
        {
            get
            {
                if (zone != ZoneType.Unknown)
                    return zone;

                zone = ZoneType.Unknown;
                if (Position.y >= 0f && Position.y < 500f)
                    zone = ZoneType.Light;
                else if (Name.Contains("EZ") || Name.Contains("INTERCOM"))
                    zone = ZoneType.Office;
                else if (Position.y < -100 && Position.y > -1015f)
                    zone = ZoneType.Heavy;
                else if (Position.y >= 5)
                    zone = ZoneType.Surface;
                return zone;
            }
        }
        public bool LightsDisabled => _lights.Length > 0 && _lights.Any(x => !x.NetworkLightsEnabled);

        public void LightsOff(float duration)
        {
            foreach (var _light in _lights)
                _light.ServerFlickerLights(duration);
        }

        internal Room(RoomIdentifier identifier)
        {
            Identifier = identifier;
            if (Identifier is null) return;
            RoomName = Identifier.Name;
            Shape = Identifier.Shape;
            GameObject = identifier.gameObject;

            _lights = GameObject.GetComponentsInChildren<RoomLightController>();

            if (_lights.Length == 0)
                defaultColor = Color.white;
            else
                defaultColor = _lights[0].OverrideColor;

            foreach (var cam in GameObject.GetComponentsInChildren<Scp079Camera>())
                Cameras.Add(new Camera(cam, this));

            Lights = new(this);

            Type = GetType(Name, Transform);

            NetworkIdentity = GetNetworkIdentity();
        }

        static internal readonly List<NetworkIdentity> NetworkIdentities = new();
        NetworkIdentity GetNetworkIdentity()
        {
            if (Type is not RoomType.Lcz330 and not RoomType.HczTestroom)
                return null;

            if (NetworkIdentities.Count == 0)
                NetworkIdentities.AddRange(Object.FindObjectsOfType<NetworkIdentity>().Where(x => x.name.Contains("All")));

            if (NetworkIdentities.TryFind(out var ident, x => Vector3.Distance(x.transform.position, Position) < 0.1f))
                return ident;

            return null;
        }

        static RoomType GetType(string name, Transform trans)
        {
            var rawName = name;
            var arr = rawName.IndexOf('(') - 1;
            if (arr > 0) rawName = rawName.Remove(arr, rawName.Length - arr).Trim();

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

                "HCZ_Room3" => RoomType.HczThreeWay,
                "HCZ_Crossing" => RoomType.HczCrossing,
                "HCZ_Curve" => RoomType.HczCurve,
                "HCZ_Straight" => RoomType.HczStraight,
                "HCZ_Tesla" => RoomType.HczTesla,
                "HCZ_Room3ar" => RoomType.HczArmory,
                "HCZ_Hid" => RoomType.HczHid,
                "HCZ_Nuke" => RoomType.HczNuke,
                "HCZ_Servers" => RoomType.HczServers,
                "HCZ_ChkpA" => RoomType.HczChkpA,
                "HCZ_ChkpB" => RoomType.HczChkpB,
                "HCZ_049" => RoomType.Hcz049,
                "HCZ_079" => RoomType.Hcz079,
                "HCZ_457" => RoomType.Hcz096,
                "HCZ_106" => RoomType.Hcz106,
                "HCZ_939" => RoomType.Hcz939,
                "HCZ Part" => RoomType.HczPart,
                "HCZ_Testroom" => RoomType.HczTestroom,

                "EZ_ThreeWay" => RoomType.EzThreeWay,
                "EZ_Crossing" => RoomType.EzCrossing,
                "EZ_Curve" => RoomType.EzCurve,
                "EZ_Straight" => RoomType.EzStraight,
                "EZ_Cafeteria" => RoomType.EzCafeteria,
                "EZ_upstairs" => RoomType.EzUpstairsPcs,
                "EZ_PCs_small" => RoomType.EzDownstairsPcs,
                "EZ_PCs" => RoomType.EzPcs,
                "EZ_Smallrooms2" => RoomType.EzSmall,
                "EZ_Intercom" => RoomType.EzIntercom,
                "EZ_Shelter" => RoomType.EzShelter,
                "EZ_Endoof" => RoomType.EzVent,
                "EZ_GateA" => RoomType.EzGateA,
                "EZ_GateB" => RoomType.EzGateB,
                "EZ Part" => trans.parent.name switch
                {
                    "HCZ_EZ_Checkpoint (A)" => RoomType.HczChkpA,
                    "HCZ_EZ_Checkpoint (B)" => RoomType.HczChkpB,
                    _ => RoomType.EzPart,
                },
                "EZ_CollapsedTunnel" => RoomType.EzCollapsedTunnel,

                "Outside" => RoomType.Surface,
                "PocketWorld" => RoomType.Pocket,

                _ => RoomType.Unknown,
            };
        }
    }
}