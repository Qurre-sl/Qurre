using System;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons.Models;

[PublicAPI]
public class ModelLight
{
    public ModelLight(Model model, Color color, Vector3 position, float lightIntensivity = 5, float lightRange = 10,
        bool shadows = true)
    {
        Light = new LightPoint(position, color, lightIntensivity, lightRange, shadows);
        GameObject = Light.Base.gameObject;

        try
        {
            GameObject.transform.parent = model.GameObject.transform;
            GameObject.transform.localPosition = position;
        }
        catch (Exception ex)
        {
            Log.Warn($"{ex}\n{ex.StackTrace}");
        }
    }

    public GameObject GameObject { get; }

    public LightPoint Light { get; }
}