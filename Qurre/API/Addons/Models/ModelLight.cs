using UnityEngine;
using System;
using Qurre.API.Controllers;

namespace Qurre.API.Addons.Models
{
    public class ModelLight
    {
        private readonly GameObject gameObject;
        private readonly LightPoint light;

        public GameObject GameObject => gameObject;
        public LightPoint Light => light;

        public ModelLight(Model model, Color color, Vector3 position, float lightIntensivity = 1, float lightRange = 10, bool shadows = true)
        {
            try
            {
                light = new(position, color, lightIntensivity, lightRange, shadows);
                gameObject = Light.Base.gameObject;

                GameObject.transform.parent = model?.GameObject?.transform;
                GameObject.transform.localPosition = position;
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }
    }
}