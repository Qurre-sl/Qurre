using Qurre.API.Controllers;
using Qurre.API.Objects;
using UnityEngine;
using System;
using Interactables.Interobjects.DoorUtils;
using Mirror;

namespace Qurre.API.Addons.Models
{
    public class ModelDoor
    {
        private readonly GameObject gameObject;
        private readonly Door door;

        public GameObject GameObject => gameObject;
        public Door Door => door;

        public ModelDoor(Model model, DoorPrefabs type, Vector3 position, Vector3 rotation,
            Vector3 size = default, DoorPermissions permissions = null)
        {
            try
            {
                door = new(position, type, permissions: permissions);
                gameObject = Door.GameObject;

                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.parent = model?.GameObject?.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
                NetworkServer.Spawn(GameObject);

                Door.DoorVariant.netIdentity.UpdateData();

                Door.DoorVariant.netIdentity.gameObject.transform.parent = GameObject.transform.parent;
                Door.DoorVariant.netIdentity.gameObject.transform.localPosition = GameObject.transform.localPosition;
                Door.DoorVariant.netIdentity.gameObject.transform.localRotation = GameObject.transform.localRotation;
                Door.DoorVariant.netIdentity.gameObject.transform.localScale = GameObject.transform.localScale;
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }
    }
}