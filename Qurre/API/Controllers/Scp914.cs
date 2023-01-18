using Scp914;
using UnityEngine;
using Utils.ConfigHandler;

namespace Qurre.API.Controllers
{
    public static class Scp914
    {
        public static Scp914Controller Controller { get; internal set; }
        public static GameObject GameObject => Controller.gameObject;
        public static bool Working => Controller._isUpgrading;

        public static Scp914KnobSetting KnobState
        {
            get => Controller._knobSetting;
            set => Controller.Network_knobSetting = value;
        }

        public static ConfigEntry<Scp914Mode> Config
        {
            get => Controller._configMode;
            set => Controller._configMode = value;
        }

        public static Transform Intake
        {
            get => Controller.IntakeChamber;
            set => Controller.IntakeChamber = value;
        }

        public static Transform Output
        {
            get => Controller.OutputChamber;
            set => Controller.OutputChamber = value;
        }

        public static void Activate() => Controller.ServerInteract(Server.Host.ReferenceHub, 0);
    }
}