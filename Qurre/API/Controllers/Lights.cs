using Qurre.API.Addons.Models;
using System.Linq;
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
            get => _room is null || _room._lights.Any(x => x.NetworkOverrideColor != _room.defaultColor);
            set
            {
                if (LockChange)
                {
                    Log.Debug($"Lights locked. Called field.set: [Override]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    return;
                }

                if (_room is not null)
                {
                    if (value) Log.Debug($"Override.set = true is not supported in Default Room. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    else foreach (var light in _room._lights) light.NetworkOverrideColor = _room.defaultColor;
                }
                else if (_custom is not null)
                {
                    if (value) Log.Debug($"Override.set = true is not supported in Custom Room. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    else foreach (var light in _custom._colors) light.Key.Light.Color = light.Value;
                }
            }
        }

        public float Intensity
        {
            get => _room is not null ? 1 : _custom._intensity;
            set
            {
                if (LockChange)
                {
                    Log.Debug($"Lights locked. Called field.set: [Intensity]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    return;
                }

                if (_room is not null)
                {
                    Log.Debug($"Lights Intensity.set doesnt support on default rooms. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    return;
                }
                else if (_custom is not null)
                {
                    _custom._intensity = value;
                    foreach (var light in _custom.Lights) light.Light.Intensivity = value;
                }
            }
        }

        public Color Color
        {
            get => _room is not null ? (_room._lights.Length > 0 ? _room._lights[0].NetworkOverrideColor : _room.defaultColor) : _custom._lastColor;
            set
            {
                if (LockChange)
                {
                    Log.Debug($"Lights locked. Called field.set: [Color]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    return;
                }

                if (_room is not null)
                {
                    foreach (var light in _room._lights)
                        light.NetworkOverrideColor = value;
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