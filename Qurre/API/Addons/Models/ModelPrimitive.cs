using System;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons.Models;

[PublicAPI]
public class ModelPrimitive
{
    public ModelPrimitive(Model model, PrimitiveType primitiveType, Color color, Vector3 position,
        Vector3 rotation, Vector3 size = default, bool collider = true)
    {
        Primitive = new Primitive(primitiveType, position, color);
        GameObject = Primitive.Base.gameObject;

        try
        {
            GameObject.transform.parent = model.GameObject.transform;
            GameObject.transform.localPosition = position;
            GameObject.transform.localRotation = Quaternion.Euler(rotation);
            GameObject.transform.localScale = size;
            Primitive.Collider = collider;

            Primitive.Base.NetworkScale = GameObject.transform.lossyScale;
            Primitive.Base.NetworkPosition = GameObject.transform.position;
            Primitive.Base.NetworkRotation = GameObject.transform.rotation;
        }
        catch (Exception ex)
        {
            Log.Warn($"{ex}\n{ex.StackTrace}");
        }
    }

    public GameObject GameObject { get; }

    public Primitive Primitive { get; }
}