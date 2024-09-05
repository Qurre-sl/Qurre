using Qurre.API.Addons.Models;
using Qurre.API.Objects;
using System.Linq;
using UnityEngine;

namespace Qurre.API.Controllers
{
    static public class GlobalLights
    {
        static public void TurnOff(float duration)
        {
            foreach (var room in Map.Rooms)
                room.LightsOff(duration);
        }
        static public void TurnOff(float duration, ZoneType zone)
        {
            foreach (var room in Map.Rooms.Where(x => x.Zone == zone))
                room.LightsOff(duration);
        }

        static public void ChangeColor(Color color, bool customToo = true, bool lockChange = false, bool ignoreLock = false)
        {
            foreach (var room in Map.Rooms)
            {
                if (ignoreLock) room.Lights.LockChange = false;
                room.Lights.Color = color;
                if (lockChange) room.Lights.LockChange = true;
            }

            if (customToo) foreach (var room in CustomRoom._list)
                {
                    if (ignoreLock) room.LightsController.LockChange = false;
                    room.LightsController.Color = color;
                    if (lockChange) room.LightsController.LockChange = true;
                }
        }
        static public void ChangeColor(Color color, ZoneType zone)
        {
            foreach (var room in Map.Rooms.Where(x => x.Zone == zone))
                room.Lights.Color = color;
        }

        static public void Intensivity(float intensive, bool customToo = false)
        {
            foreach (var room in Map.Rooms)
                room.Lights.Intensity = intensive;

            if (customToo) foreach (var room in CustomRoom._list)
                    room.LightsController.Intensity = intensive;
        }
        static public void Intensivity(float intensive, ZoneType zone)
        {
            foreach (var room in Map.Rooms.Where(x => x.Zone == zone))
                room.Lights.Intensity = intensive;
        }

        static public void SetToDefault(bool customToo = true, bool ignoreLock = false)
        {
            foreach (var room in Map.Rooms)
            {
                if (ignoreLock) room.Lights.LockChange = false;
                room.Lights.Override = false;
            }

            if (customToo) foreach (var room in CustomRoom._list)
                {
                    if (ignoreLock) room.LightsController.LockChange = false;
                    room.LightsController.Override = false;
                }
        }
    }
}