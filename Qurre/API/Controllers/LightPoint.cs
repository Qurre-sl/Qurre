using System;
using AdminToys;
using Footprinting;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Addons;
using Qurre.API.Controllers.Components;
using Qurre.API.World;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class LightPoint : AdminToy<LightSourceToy>
{
    public LightPoint(Vector3 position, Color lightColor = default, float lightIntensity = 5.0F,
        float lightRange = 10.0F, LightType lightType = LightType.Point, Quaternion rotation = default,
        LightShadows shadowType = LightShadows.None, float shadowStrength = 1.0F)
    {
        if (Prefabs.Light == null)
            throw new NullReferenceException(nameof(Prefabs.Light));

        if (!Prefabs.Light.TryGetComponent<LightSourceToy>(out LightSourceToy? lightToyBase))
            throw new Exception("LightSourceToy not found");

        Base = Object.Instantiate(lightToyBase);
        Base.SpawnerFootprint = new Footprint(Server.Host.ReferenceHub);
        NetworkServer.Spawn(Base.gameObject);

        Position = position;
        Rotation = rotation;
        Scale = Vector3.one;

        if (lightColor == default || lightColor is { r: < 0.1f, g: < 0.1f, b: < 0.1f })
            lightColor = Color.white;

        Color = lightColor;
        Intensity = lightIntensity;
        Range = lightRange;
        Type = lightType;
        ShadowType = shadowType;
        ShadowStrength = shadowStrength;

        Map.Lights.Add(this);
    }

    public Color Color
    {
        get => Base.NetworkLightColor;
        set => Base.NetworkLightColor = value;
    }

    public float Intensity
    {
        get => Base.NetworkLightIntensity;
        set => Base.NetworkLightIntensity = value;
    }

    public float Range
    {
        get => Base.NetworkLightRange;
        set => Base.NetworkLightRange = value;
    }

    public LightType Type
    {
        get => Base.NetworkLightType;
        set => Base.NetworkLightType = value;
    }

    public LightShadows ShadowType
    {
        get => Base.NetworkShadowType;
        set => Base.NetworkShadowType = value;
    }

    public float ShadowStrength
    {
        get => Base.NetworkShadowStrength;
        set => Base.NetworkShadowStrength = value;
    }

    public LightShape Shape
    {
        get => Base.NetworkLightShape;
        set => Base.NetworkLightShape = value;
    }

    public float SpotAngle
    {
        get => Base.NetworkSpotAngle;
        set => Base.NetworkSpotAngle = value;
    }

    public float InnerSpotAngle
    {
        get => Base.NetworkInnerSpotAngle;
        set => Base.NetworkInnerSpotAngle = value;
    }

    public override void Destroy()
    {
        NetworkServer.Destroy(Base.gameObject);
        Map.Lights.Remove(this);
    }
}