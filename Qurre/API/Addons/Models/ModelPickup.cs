using UnityEngine;
using System;
using Mirror;
using Object = UnityEngine.Object;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;

namespace Qurre.API.Addons.Models
{
    public class ModelPickup
    {
        private readonly GameObject gameObject;
        private readonly ItemPickupBase pickup;

        public GameObject GameObject => gameObject;
        public ItemPickupBase Pickup => pickup;

        public ModelPickup(Model model, ItemType type, Vector3 position, Vector3 rotation, Vector3 size = default, bool kinematic = true)
        {
            try
            {
                var item = Server.InventoryHost.CreateItemInstance(new(type, ItemSerialGenerator.GenerateNext()), true);
                ushort ser = ItemSerialGenerator.GenerateNext();

                item.PickupDropModel.Info.Serial = ser;
                item.PickupDropModel.Info.ItemId = type;
                item.PickupDropModel.Info._serverPosition = model.GameObject.transform.position + position;
                item.PickupDropModel.Info.Weight = item.Weight;
                item.PickupDropModel.Info._serverRotation = Quaternion.Euler(model.GameObject.transform.rotation.eulerAngles + rotation);
                item.PickupDropModel.NetworkInfo = item.PickupDropModel.Info;

                ItemPickupBase ipb = Object.Instantiate(item.PickupDropModel, item.PickupDropModel.Info._serverPosition, item.PickupDropModel.Info._serverRotation);

                gameObject = ipb.gameObject;
                GameObject.GetComponent<Rigidbody>().isKinematic = kinematic;
                GameObject.transform.parent = model.GameObject.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
                ipb.NetworkInfo = item.PickupDropModel.NetworkInfo;
                NetworkServer.Spawn(GameObject);
                ipb.InfoReceived(default, ipb.NetworkInfo);

                pickup = ipb;
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }
    }
}