using Qurre.API.Addons.Models;
using System.Reflection;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Lights
    {
        readonly Room _room;
        readonly CustomRoom _custom;

#nullable enable
        public Room? Room => _room;
        public CustomRoom? CustomRoom => _custom;
#nullable disable

        public bool LockChange { get; set; } = false;
        public bool Override
        {
            get => _room is null || _room._light.WarheadLightOverride;
            set
            {
                if (LockChange)
                {
                    Log.Debug($"Lights locked. Called field.set: [Override]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    return;
                }

                if (_room is not null) _room._light.WarheadLightOverride = value;
                else if (_custom is not null)
                {
                    if (value) Log.Debug($"Override.set = true is not supported in Custom Room. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    else foreach (var light in _custom._colors) light.Key.Light.Color = light.Value;
                }
            }
        }

        public float Intensity
        {
            get => _room is not null ? _room._light.LightIntensityMultiplier : _custom._intensity;
            set
            {
                if (LockChange)
                {
                    Log.Debug($"Lights locked. Called field.set: [Intensity]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    return;
                }

                if (_room is not null) _room._light.UpdateLightsIntensity(_room._light.LightIntensityMultiplier, value);
                else if (_custom is not null)
                {
                    _custom._intensity = value;
                    foreach (var light in _custom.Lights) light.Light.Intensivity = value;
                }
            }
        }

        public Color Color
        {
            get => _room is not null ? _room._light.Network_warheadLightColor : _custom._lastColor;
            set
            {
                if (LockChange)
                {
                    Log.Debug($"Lights locked. Called field.set: [Color]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    return;
                }

                if (_room is not null)
                {
                    _room._light.Network_warheadLightColor = value;
                    Override = true;
                }
                else if (_custom is not null)
                {
                    foreach (var light in _custom.Lights) light.Light.Color = value;
                    _custom._lastColor = value;
                }
            }
        }

        internal Lights(Room room) => _room = room;
        internal Lights(CustomRoom room) => _custom = room;
    }
}