using Qurre.API.Controllers;
using UnityEngine;
using Mirror;

namespace Qurre.API.Addons.Models
{
    public class ModelWorkStation
    {
        private readonly GameObject gameObject;
        private readonly WorkStation workStation;

        public GameObject GameObject => gameObject;
        public WorkStation WorkStation => workStation;

        public ModelWorkStation(Model model, Vector3 position, Vector3 rotation, Vector3 size = default)
        {
            try
            {
                GameObject transformer = new("GetPosition");
                transformer.transform.parent = model.GameObject.transform;
                transformer.transform.localPosition = position;
                transformer.transform.localRotation = Quaternion.Euler(rotation);
                transformer.transform.localScale = size;

                workStation = new(transformer.transform.position, transformer.transform.rotation.eulerAngles, transformer.transform.lossyScale);
                gameObject = WorkStation.GameObject;

                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.parent = model.GameObject.transform;
                GameObject.transform.position = transformer.transform.position;
                GameObject.transform.rotation = transformer.transform.rotation;
                GameObject.transform.localScale = transformer.transform.lossyScale;
                NetworkServer.Spawn(GameObject);

                WorkStation.workStation.netIdentity.UpdateData();

                Object.Destroy(transformer);
            }
            catch (System.Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }
    }
}