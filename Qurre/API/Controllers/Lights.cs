using System.Collections.Generic;
using System.Reflection;
using Qurre.API.Addons.Models;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Lights
    {
        internal Lights(Room room) => Room = room;

        internal Lights(CustomRoom room) => CustomRoom = room;

        public bool LockChange { get; set; } = false;

        public bool Override
        {
            get => Room is null || Room._light.WarheadLightOverride;
            set
            {
                if (LockChange)
                {
                    Log.Debug($"Lights locked. Called field.set: [Override]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    return;
                }

                if (Room is not null)
                {
                    Room._light.WarheadLightOverride = value;
                }
                else if (CustomRoom is not null)
                {
                    if (value)
                    {
                        Log.Debug($"Override.set = true is not supported in Custom Room. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    }
                    else
                    {
                        foreach (KeyValuePair<ModelLight, Color> light in CustomRoom._colors)
                        {
                            light.Key.Light.Color = light.Value;
                        }
                    }
                }
            }
        }

        public float Intensity
        {
            get => Room is not null ? Room._light.LightIntensityMultiplier : CustomRoom._intensity;
            set
            {
                if (LockChange)
                {
                    Log.Debug($"Lights locked. Called field.set: [Intensity]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    return;
                }

                if (Room is not null)
                {
                    Room._light.UpdateLightsIntensity(Room._light.LightIntensityMultiplier, value);
                }
                else if (CustomRoom is not null)
                {
                    CustomRoom._intensity = value;

                    foreach (ModelLight light in CustomRoom.Lights)
                    {
                        light.Light.Intensivity = value;
                    }
                }
            }
        }

        public Color Color
        {
            get => Room is not null ? Room._light.Network_warheadLightColor : CustomRoom._lastColor;
            set
            {
                if (LockChange)
                {
                    Log.Debug($"Lights locked. Called field.set: [Color]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                    return;
                }

                if (Room is not null)
                {
                    Room._light.Network_warheadLightColor = value;
                    Override = true;
                }
                else if (CustomRoom is not null)
                {
                    foreach (ModelLight light in CustomRoom.Lights)
                    {
                        light.Light.Color = value;
                    }

                    CustomRoom._lastColor = value;
                }
            }
        }

#nullable enable
        public Room? Room { get; }

        public CustomRoom? CustomRoom { get; }
#nullable disable
    }
}