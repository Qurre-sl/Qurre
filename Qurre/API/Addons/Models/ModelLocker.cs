using System;
using Mirror;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using UnityEngine;

namespace Qurre.API.Addons.Models
{
    public class ModelLocker
    {
        public ModelLocker(Model model, LockerPrefabs type, Vector3 position, Vector3 rotation, Vector3 size = default)
        {
            try
            {
                Locker = new (position, type);
                GameObject = Locker.GameObject;

                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.parent = model?.GameObject?.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
                NetworkServer.Spawn(GameObject);
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }

        public GameObject GameObject { get; }

        public Locker Locker { get; }
    }
}