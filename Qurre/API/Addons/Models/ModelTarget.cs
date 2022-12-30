using Qurre.API.Controllers;
using UnityEngine;
using System;
using Qurre.API.Objects;

namespace Qurre.API.Addons.Models
{
    public class ModelTarget
    {
        private readonly GameObject gameObject;
        private readonly ShootingTarget target;

        public GameObject GameObject => gameObject;
        public ShootingTarget Target => target;

        public ModelTarget(Model model, TargetPrefabs prefab, Vector3 position, Vector3 rotation, Vector3 size)
        {
            try
            {
                target = new(prefab, position);
                gameObject = Target.Base.gameObject;

                GameObject.transform.parent = model?.GameObject?.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }
    }
}