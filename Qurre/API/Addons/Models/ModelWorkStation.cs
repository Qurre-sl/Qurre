using System;
using Mirror;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons.Models
{
    public class ModelWorkStation
    {
        public ModelWorkStation(Model model, Vector3 position, Vector3 rotation, Vector3 size = default)
        {
            try
            {
                WorkStation = new (position, Vector3.zero, Vector3.one);
                GameObject = WorkStation.GameObject;

                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.parent = model?.GameObject?.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
                NetworkServer.Spawn(GameObject);

                WorkStation.workStation.netIdentity.UpdateData();
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }

        public GameObject GameObject { get; }

        public WorkStation WorkStation { get; }
    }
}