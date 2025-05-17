using System;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Controllers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Addons.Models;

[PublicAPI]
public class ModelWorkStation
{
    public ModelWorkStation(Model model, Vector3 position, Vector3 rotation, Vector3 size = default)
    {
        GameObject transformer = new("GetPosition")
        {
            transform =
            {
                parent = model.GameObject.transform,
                localPosition = position,
                localRotation = Quaternion.Euler(rotation),
                localScale = size
            }
        };

        WorkStation = new WorkStation(transformer.transform.position, transformer.transform.rotation.eulerAngles,
            transformer.transform.lossyScale);
        GameObject = WorkStation.GameObject;

        try
        {
            NetworkServer.UnSpawn(GameObject);
            GameObject.transform.parent = model.GameObject.transform;
            GameObject.transform.position = transformer.transform.position;
            GameObject.transform.rotation = transformer.transform.rotation;
            GameObject.transform.localScale = transformer.transform.lossyScale;
            NetworkServer.Spawn(GameObject);

            WorkStation.Controller.netIdentity.UpdateData();
        }
        catch (Exception ex)
        {
            Log.Warn($"{ex}\n{ex.StackTrace}");
        }

        Object.Destroy(transformer);
    }

    public GameObject GameObject { get; }

    public WorkStation WorkStation { get; }
}