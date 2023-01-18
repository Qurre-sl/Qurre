using System;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using UnityEngine;

namespace Qurre.API.Addons.Models
{
    public class ModelDoor
    {
        public ModelDoor(Model model, DoorPrefabs type, Vector3 position, Vector3 rotation,
            Vector3 size = default, DoorPermissions permissions = null)
        {
            try
            {
                Door = new (position, type, permissions: permissions);
                GameObject = Door.GameObject;

                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.parent = model?.GameObject?.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
                NetworkServer.Spawn(GameObject);

                Door.DoorVariant.netIdentity.UpdateData();
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }

        public GameObject GameObject { get; }

        public Door Door { get; }
    }
}