using Qurre.API.Controllers;
using Qurre.API.Objects;
using UnityEngine;
using System;
using Mirror;

namespace Qurre.API.Addons.Models
{
    public class ModelLocker
    {
        private readonly GameObject gameObject;
        private readonly Locker locker;

        public GameObject GameObject => gameObject;
        public Locker Locker => locker;

        public ModelLocker(Model model, LockerPrefabs type, Vector3 position, Vector3 rotation, Vector3 size = default)
        {
            try
            {
                locker = new(position, type);
                gameObject = Locker.GameObject;

                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.parent = model?.GameObject?.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
                NetworkServer.Spawn(GameObject);

                locker._locker.netIdentity.UpdateData();
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }
    }
}