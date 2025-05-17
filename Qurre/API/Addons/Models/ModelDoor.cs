using System;
using Interactables.Interobjects.DoorUtils;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using UnityEngine;

namespace Qurre.API.Addons.Models;

[PublicAPI]
public class ModelDoor
{
    public ModelDoor(Model model, DoorPrefabs type, Vector3 position, Vector3 rotation,
        Vector3 size = default, DoorPermissionsPolicy? permissions = null)
    {
        Door = new Door(position, type, permissions: permissions);
        GameObject = Door.GameObject;

        try
        {
            NetworkServer.UnSpawn(GameObject);
            GameObject.transform.parent = model.GameObject.transform;
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

    public GameObject GameObject { get; }

    public Door Door { get; }
}