using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons.Models;

[PublicAPI]
public class CustomRoom : Model
{
    private static readonly List<CustomRoom> LocalList = [];
    private Dictionary<ModelLight, Color> _colors = [];

    internal float Intensity = 1;
    internal Color LastColor = Color.white;

    public CustomRoom(string id, Vector3 position, Vector3 rotation = default, Model? root = null) : this(id, position,
        rotation, Vector3.one, root)
    {
    }

    public CustomRoom(string id, Vector3 position, Vector3 rotation, Vector3 scale, Model? root = null)
        : base(id, position, rotation, scale, root)
    {
        LightsController = new Lights(this);
        LocalList.Add(this);
    }

    public static IReadOnlyCollection<CustomRoom> List => LocalList.AsReadOnly();
    public IReadOnlyDictionary<ModelLight, Color> Colors => _colors;

    public Lights LightsController { get; private set; }

    public new void AddPart(ModelLight light, bool addToList = true)
    {
        if (addToList)
        {
            Intensity = Math.Max(Intensity, light.Light.Intensity);
            _colors.Add(light, light.Light.Color);
            LastColor = light.Light.Color;
        }

        base.AddPart(light, addToList);
    }
}