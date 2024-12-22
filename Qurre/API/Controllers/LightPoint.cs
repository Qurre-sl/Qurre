using System;
using AdminToys;
using Footprinting;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Addons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class LightPoint
{
    public LightSourceToy Base { get; }

    public Vector3 Position
    {
        get => Base.transform.position;
        set
        {
            Base.transform.position = value;
            Base.NetworkPosition = value;
        }
    }

    public Quaternion RotationQuaternion
    {
        get => Base.transform.localRotation;
        set
        {
            Base.transform.rotation = value;
            Base.NetworkRotation = value;
        }
    }

    public Vector3 RotationEuler
    {
        get => Base.transform.localRotation.eulerAngles;
        set
        {
            var quaternion = Quaternion.Euler(value);
            Base.transform.rotation = quaternion;
            Base.NetworkRotation = quaternion;
        }
    }

    public Vector3 Scale
    {
        get => Base.transform.localScale;
        set
        {
            Base.transform.localScale = value;
            Base.NetworkScale = Base.transform.lossyScale;
        }
    }

    public Vector3 GlobalScale => Base.transform.lossyScale;

    public byte MovementSmoothing
    {
        get => Base.NetworkMovementSmoothing;
        set => Base.NetworkMovementSmoothing = value;
    }

    public bool IsStatic
    {
        get => Base.NetworkIsStatic;
        set
        {
            if (value)
            {
                Base.NetworkPosition = Base.transform.position;
                Base.NetworkRotation = Base.transform.rotation;
                Base.NetworkScale = Base.transform.lossyScale;
            }

            Base.NetworkIsStatic = value;
        }
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

    public LightPoint(Vector3 position, Color lightColor = default, float lightIntensity = 5.0F,
        float lightRange = 10.0F, LightType lightType = LightType.Point, Quaternion rotation = default,
        LightShadows shadowType = LightShadows.None, float shadowStrength = 1.0F)
    {
        if (Prefabs.Light == null)
            throw new NullReferenceException(nameof(Prefabs.Light));

        if (!Prefabs.Light.TryGetComponent<LightSourceToy>(out var lightToyBase))
            throw new Exception("LightSourceToy not found");

        Base = Object.Instantiate(lightToyBase);
        Base.SpawnerFootprint = new Footprint(Server.Host.ReferenceHub);
        NetworkServer.Spawn(Base.gameObject);

        Position = position;
        RotationQuaternion = rotation;
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

    public void Destroy()
    {
        NetworkServer.Destroy(Base.gameObject);
        Map.Lights.Remove(this);
    }
}