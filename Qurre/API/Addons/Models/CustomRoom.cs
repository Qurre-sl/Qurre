using System;
using System.Collections.Generic;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons.Models
{
    public class CustomRoom : Model
    {
        internal static readonly List<CustomRoom> _list = new ();

        internal float _intensity = 1;
        internal Color _lastColor = Color.white;
        internal Dictionary<ModelLight, Color> _colors = new ();

        public CustomRoom(string id, Vector3 position, Vector3 rotation = default, Model root = null) : base(id, position, rotation, root)
        {
            LightsController = new (this);
            _list.Add(this);
            lights.ForEach(
                light =>
                {
                    _intensity = Math.Max(_intensity, light.Light.Intensivity);
                    _colors.Add(light, light.Light.Color);
                    _lastColor = light.Light.Color;
                });
        }

        public static IReadOnlyCollection<CustomRoom> List => _list.AsReadOnly();

        public Lights LightsController { get; }

        public new void AddPart(ModelLight part, bool addToList = true)
        {
            if (addToList)
            {
                lights.Add(part);
                _intensity = Math.Max(_intensity, part.Light.Intensivity);
                _colors.Add(part, part.Light.Color);
                _lastColor = part.Light.Color;
            }

            parts.Add(part.GameObject, ModelEnums.Light);
        }
    }
}