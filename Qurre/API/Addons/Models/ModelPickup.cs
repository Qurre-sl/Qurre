using System;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Addons.Models;

[PublicAPI]
public class ModelPickup
{
    public ModelPickup(Model model, ItemType type, Vector3 position, Vector3 rotation, Vector3 size = default,
        bool kinematic = true)
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

        Pickup = Pickup.Create(type, transformer.transform.position, transformer.transform.rotation,
            transformer.transform.lossyScale);

        if (Pickup is null)
            throw new NullReferenceException(nameof(Pickup));

        GameObject = Pickup.GameObject;

        try
        {
            if (kinematic)
                GameObject.GetComponent<Rigidbody>().isKinematic = kinematic;
        }
        catch (Exception ex)
        {
            Log.Warn($"{ex}\n{ex.StackTrace}");
        }

        Object.Destroy(transformer);
    }

    public GameObject GameObject { get; }

    public Pickup? Pickup { get; }
}