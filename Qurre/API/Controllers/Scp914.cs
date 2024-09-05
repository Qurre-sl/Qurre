using Scp914;
using UnityEngine;
using Utils.ConfigHandler;

namespace Qurre.API.Controllers
{
	static public class Scp914
	{
		static public Scp914Controller Controller { get; internal set; }
		static public GameObject GameObject => Controller.gameObject;
		static public bool Working => Controller._isUpgrading;

		static public Scp914KnobSetting KnobState
		{
			get => Controller._knobSetting;
			set => Controller.Network_knobSetting = value;
		}
		static public ConfigEntry<Scp914Mode> Config
		{
			get => Controller._configMode;
			set => Controller._configMode = value;
		}

		static public Transform Intake
		{
			get => Controller.IntakeChamber;
			set => Controller.IntakeChamber = value;
		}
		static public Transform Output
		{
			get => Controller.OutputChamber;
			set => Controller.OutputChamber = value;
		}

		static public void Activate() => Controller.ServerInteract(Server.Host.ReferenceHub, 0);
	}
}