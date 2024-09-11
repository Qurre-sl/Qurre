using System;
using AdminToys;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Addons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class LightPoint
{
    public LightPoint(Vector3 position, Color lightColor = default, float lightIntensivity = 5, float lightRange = 10,
        bool shadows = true)
    {
        if (Prefabs.Light == null)
            throw new NullReferenceException(nameof(Prefabs.Light));

        if (!Prefabs.Light.TryGetComponent<LightSourceToy>(out LightSourceToy? lightToyBase))
            throw new Exception("LightSourceToy not found");

        Base = Object.Instantiate(lightToyBase);

        Base.transform.position = position;
        Base.transform.localScale = Vector3.one;

        NetworkServer.Spawn(Base.gameObject);

        if (lightColor == default) lightColor = Color.white;
        if (lightColor is { r: < 0.1f, g: < 0.1f, b: < 0.1f })
            lightColor = Color.white;

        Color = lightColor;
        Intensivity = lightIntensivity;
        Range = lightRange;
        EnableShadows = shadows;

        Map.Lights.Add(this);
    }

    public LightSourceToy Base { get; }

    public Vector3 Position
    {
        get => Base.transform.position;
        set
        {
            NetworkServer.UnSpawn(Base.gameObject);
            Base.transform.position = value;
            NetworkServer.Spawn(Base.gameObject);
        }
    }

    public Vector3 Scale
    {
        get => Base.transform.localScale;
        set
        {
            NetworkServer.UnSpawn(Base.gameObject);
            Base.transform.localScale = value;
            NetworkServer.Spawn(Base.gameObject);
        }
    }

    public Quaternion Rotation
    {
        get => Base.transform.localRotation;
        set
        {
            NetworkServer.UnSpawn(Base.gameObject);
            Base.transform.localRotation = value;
            NetworkServer.Spawn(Base.gameObject);
        }
    }

    public Color Color
    {
        get => Base.LightColor;
        set => Base.NetworkLightColor = value;
    }

    public float Intensivity
    {
        get => Base.LightIntensity;
        set => Base.NetworkLightIntensity = value;
    }

    public float Range
    {
        get => Base.LightRange;
        set => Base.NetworkLightRange = value;
    }

    public bool EnableShadows
    {
        get => Base.LightShadows;
        set => Base.NetworkLightShadows = value;
    }

    public void Destroy()
    {
        NetworkServer.Destroy(Base.gameObject);
        Map.Lights.Remove(this);
    }
}