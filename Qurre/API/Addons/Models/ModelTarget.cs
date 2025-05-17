using System;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using UnityEngine;

namespace Qurre.API.Addons.Models;

[PublicAPI]
public class ModelTarget
{
    public ModelTarget(Model model, TargetPrefabs prefab, Vector3 position, Vector3 rotation, Vector3 size)
    {
        Target = new ShootingTarget(prefab, position);
        GameObject = Target.Base.gameObject;

        try
        {
            GameObject.transform.parent = model.GameObject.transform;
            GameObject.transform.localPosition = position;
            GameObject.transform.localRotation = Quaternion.Euler(rotation);
            GameObject.transform.localScale = size;
        }
        catch (Exception ex)
        {
            Log.Warn($"{ex}\n{ex.StackTrace}");
        }
    }

    public GameObject GameObject { get; }

    public ShootingTarget Target { get; }
}