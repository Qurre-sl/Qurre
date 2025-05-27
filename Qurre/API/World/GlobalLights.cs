using System.Linq;
using JetBrains.Annotations;
using MapGeneration;
using Qurre.API.Addons.Models;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.World;

[PublicAPI]
public static class GlobalLights
{
    public static void TurnOff(float duration)
    {
        foreach (Room? room in Map.Rooms)
            room.LightsOff(duration);
    }

    public static void TurnOff(float duration, FacilityZone zone)
    {
        foreach (Room? room in Map.Rooms.Where(x => x.Zone == zone))
            room.LightsOff(duration);
    }

    public static void ChangeColor(Color color, bool customToo = true, bool lockChange = false, bool ignoreLock = false)
    {
        foreach (Room? room in Map.Rooms)
        {
            if (ignoreLock) room.Lights.LockChange = false;
            room.Lights.Color = color;
            if (lockChange) room.Lights.LockChange = true;
        }

        // ReSharper disable once InvertIf
        if (customToo)
            foreach (CustomRoom? room in CustomRoom.List)
            {
                if (ignoreLock) room.LightsController.LockChange = false;
                room.LightsController.Color = color;
                if (lockChange) room.LightsController.LockChange = true;
            }
    }

    public static void ChangeColor(Color color, FacilityZone zone)
    {
        foreach (Room? room in Map.Rooms.Where(x => x.Zone == zone))
            room.Lights.Color = color;
    }

    public static void Intensivity(float intensive, bool customToo = false)
    {
        foreach (Room? room in Map.Rooms)
            room.Lights.Intensity = intensive;

        // ReSharper disable once InvertIf
        if (customToo)
            foreach (CustomRoom? room in CustomRoom.List)
                room.LightsController.Intensity = intensive;
    }

    public static void Intensivity(float intensive, FacilityZone zone)
    {
        foreach (Room? room in Map.Rooms.Where(x => x.Zone == zone))
            room.Lights.Intensity = intensive;
    }

    public static void SetToDefault(bool customToo = true, bool ignoreLock = false)
    {
        foreach (Room? room in Map.Rooms)
        {
            if (ignoreLock) room.Lights.LockChange = false;
            room.Lights.Override = false;
        }

        // ReSharper disable once InvertIf
        if (customToo)
            foreach (CustomRoom? room in CustomRoom.List)
            {
                if (ignoreLock) room.LightsController.LockChange = false;
                room.LightsController.Override = false;
            }
    }
}