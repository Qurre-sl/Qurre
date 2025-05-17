using System;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons.Models;

[PublicAPI]
public class ModelLight
{
    public ModelLight(Model model, Color color, Vector3 position, float lightIntensity = 5.0F, float lightRange = 10.0F,
        LightType type = LightType.Point, Quaternion rotation = default, LightShadows shadowType = LightShadows.None,
        float shadowStrength = 1.0F)
    {
        Light = new LightPoint(position, color, lightIntensity, lightRange, type, rotation, shadowType, shadowStrength);
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