using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Qurre.API.Addons.Models;
using UnityEngine;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Lights
{
    internal Lights(Room room)
    {
        Room = room;
    }

    internal Lights(CustomRoom room)
    {
        CustomRoom = room;
    }

    public Room? Room { get; }
    public CustomRoom? CustomRoom { get; }

    public bool LockChange { get; set; }

    public bool Override
    {
        get => Room is null || Room.GameLights.Any(x => x.NetworkOverrideColor != Room.DefaultColor);
        set
        {
            if (LockChange)
            {
                Log.Debug(
                    $"Lights locked. Called field.set: [Override]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                return;
            }

            if (Room is not null)
            {
                if (value)
                    Log.Debug(
                        $"Override.set = true is not supported in Default Room. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                else
                    foreach (RoomLightController light in Room.GameLights)
                        light.NetworkOverrideColor = Room.DefaultColor;
            }
            else if (CustomRoom is not null)
            {
                if (value)
                    Log.Debug(
                        $"Override.set = true is not supported in Custom Room. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                else
                    foreach (var light in CustomRoom.Colors)
                        light.Key.Light.Color = light.Value;
            }
        }
    }

    public float Intensity
    {
        get => Room is not null ? 1 : CustomRoom?.Intensity ?? 1;
        set
        {
            if (LockChange)
            {
                Log.Debug(
                    $"Lights locked. Called field.set: [Intensity]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                return;
            }

            if (Room is not null)
            {
                Log.Debug(
                    $"Lights Intensity.set doesnt support on default rooms. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                return;
            }

            // ReSharper disable once InvertIf
            if (CustomRoom is not null)
            {
                CustomRoom.Intensity = value;
                foreach (ModelLight light in CustomRoom.Lights) light.Light.Intensity = value;
            }
        }
    }

    public Color Color
    {
        get => Room is not null
            ? Room.GameLights.Length > 0 ? Room.GameLights[0].NetworkOverrideColor : Room.DefaultColor
            : CustomRoom?.LastColor ?? Color.white;
        set
        {
            if (LockChange)
            {
                Log.Debug(
                    $"Lights locked. Called field.set: [Color]. Called from {Assembly.GetCallingAssembly().GetName().Name}");
                return;
            }

            if (Room is not null)
            {
                foreach (RoomLightController light in Room.GameLights)
                    light.NetworkOverrideColor = value;
            }
            else if (CustomRoom is not null)
            {
                foreach (ModelLight light in CustomRoom.Lights) light.Light.Color = value;
                CustomRoom.LastColor = value;
            } // end if
        } // end field_set
    } // end field
}