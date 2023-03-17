using Qurre.API.Controllers;
using UnityEngine;
using System;

namespace Qurre.API.Addons.Models
{
    public class ModelPrimitive
    {
        private readonly GameObject gameObject;
        private readonly Primitive primitive;

        public GameObject GameObject => gameObject;
        public Primitive Primitive => primitive;

        public ModelPrimitive(Model model, PrimitiveType primitiveType, Color color, Vector3 position,
            Vector3 rotation, Vector3 size = default, bool collider = true)
        {
            try
            {
                primitive = new(primitiveType, position, color);
                gameObject = Primitive.Base.gameObject;

                GameObject.transform.parent = model?.GameObject?.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
                Primitive.Collider = collider;

                Primitive.Base.NetworkScale = GameObject.transform.lossyScale;
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }
    }
}