using System;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons.Models;

[PublicAPI]
public class ModelGenerator
{
    public ModelGenerator(Model model, Vector3 position, Vector3 rotation, Vector3 size = default)
    {
        Generator = new Generator(position);
        GameObject = Generator.GameObject;

        try
        {
            NetworkServer.UnSpawn(GameObject);
            GameObject.transform.parent = model.GameObject.transform;
            GameObject.transform.localPosition = position;
            GameObject.transform.localRotation = Quaternion.Euler(rotation);
            GameObject.transform.localScale = size;
            NetworkServer.Spawn(GameObject);
        }
        catch (Exception ex)
        {
            Log.Warn($"{ex}\n{ex.StackTrace}");
        }
    }

    public GameObject GameObject { get; }

    public Generator Generator { get; }
}