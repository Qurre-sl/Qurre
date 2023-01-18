using System.Linq;
using Qurre.API.Addons.Models;
using Qurre.API.Objects;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public static class GlobalLights
    {
        public static void TurnOff(float duration)
        {
            foreach (Room room in Map.Rooms)
            {
                room.LightsOff(duration);
            }
        }

        public static void TurnOff(float duration, ZoneType zone)
        {
            foreach (Room room in Map.Rooms.Where(x => x.Zone == zone))
            {
                room.LightsOff(duration);
            }
        }

        public static void ChangeColor(Color color, bool customToo = true, bool lockChange = false, bool ignoreLock = false)
        {
            foreach (Room room in Map.Rooms)
            {
                if (ignoreLock)
                {
                    room.Lights.LockChange = false;
                }

                room.Lights.Color = color;

                if (lockChange)
                {
                    room.Lights.LockChange = true;
                }
            }

            if (customToo)
            {
                foreach (CustomRoom room in CustomRoom._list)
                {
                    if (ignoreLock)
                    {
                        room.LightsController.LockChange = false;
                    }

                    room.LightsController.Color = color;

                    if (lockChange)
                    {
                        room.LightsController.LockChange = true;
                    }
                }
            }
        }

        public static void ChangeColor(Color color, ZoneType zone)
        {
            foreach (Room room in Map.Rooms.Where(x => x.Zone == zone))
            {
                room.Lights.Color = color;
            }
        }

        public static void Intensivity(float intensive, bool customToo = false)
        {
            foreach (Room room in Map.Rooms)
            {
                room.Lights.Intensity = intensive;
            }

            if (customToo)
            {
                foreach (CustomRoom room in CustomRoom._list)
                {
                    room.LightsController.Intensity = intensive;
                }
            }
        }

        public static void Intensivity(float intensive, ZoneType zone)
        {
            foreach (Room room in Map.Rooms.Where(x => x.Zone == zone))
            {
                room.Lights.Intensity = intensive;
            }
        }

        public static void SetToDefault(bool customToo = true, bool ignoreLock = false)
        {
            foreach (Room room in Map.Rooms)
            {
                if (ignoreLock)
                {
                    room.Lights.LockChange = false;
                }

                room.Lights.Override = false;
            }

            if (customToo)
            {
                foreach (CustomRoom room in CustomRoom._list)
                {
                    if (ignoreLock)
                    {
                        room.LightsController.LockChange = false;
                    }

                    room.LightsController.Override = false;
                }
            }
        }
    }
}