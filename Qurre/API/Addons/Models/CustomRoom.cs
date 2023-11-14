using Qurre.API.Controllers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Qurre.API.Addons.Models
{
    public class CustomRoom : Model
    {
        static internal readonly List<CustomRoom> _list = new();
        static public IReadOnlyCollection<CustomRoom> List => _list.AsReadOnly();

        internal float _intensity = 1;
        internal Color _lastColor = Color.white;
        internal Dictionary<ModelLight, Color> _colors = new();

        public Lights LightsController { get; private set; }
        public CustomRoom(string id, Vector3 position, Vector3 rotation = default, Model root = null) : this(id, position, rotation, Vector3.one, root) { }
        public CustomRoom(string id, Vector3 position, Vector3 rotation, Vector3 scale, Model root = null)
            : base(id, position, rotation, scale, root)
        {
            LightsController = new(this);
            _list.Add(this);
            lights.ForEach(light =>
            {
                _intensity = Math.Max(_intensity, light.Light.Intensivity);
                _colors.Add(light, light.Light.Color);
                _lastColor = light.Light.Color;
            });
        }

        new public void AddPart(ModelLight part, bool addToList = true)
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