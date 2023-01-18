﻿using System;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons.Models
{
    public class ModelPrimitive
    {
        public ModelPrimitive(Model model, PrimitiveType primitiveType, Color color, Vector3 position,
            Vector3 rotation, Vector3 size = default, bool collider = true)
        {
            try
            {
                Primitive = new (primitiveType, position, color);
                GameObject = Primitive.Base.gameObject;

                GameObject.transform.parent = model?.GameObject?.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
                Primitive.Collider = collider;
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }

        public GameObject GameObject { get; }

        public Primitive Primitive { get; }
    }
}