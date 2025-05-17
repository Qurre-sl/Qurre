using JetBrains.Annotations;
using Scp914;
using UnityEngine;
using Utils.ConfigHandler;

namespace Qurre.API.World;

[PublicAPI]
public static class Scp914
{
    static Scp914()
    {
        Controller = Object.FindObjectOfType<Scp914Controller>();
    }

    public static Scp914Controller Controller { get; internal set; }

    public static GameObject GameObject => Controller.gameObject;
    public static bool Working => Controller.IsUpgrading;
    public static Vector3 MoveVector => Scp914Controller.MoveVector;

    public static Scp914KnobSetting KnobState
    {
        get => Controller._knobSetting;
        set => Controller.Network_knobSetting = value;
    }

    public static ConfigEntry<Scp914Mode> Config
    {
        get => Controller.ConfigMode;
        set => Controller.ConfigMode = value;
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

    public static void Activate()
    {
        Controller.ServerInteract(Server.Host.ReferenceHub, 0);
    }
}