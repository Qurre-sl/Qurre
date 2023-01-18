using System;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Addons.Models
{
    public class ModelPickup
    {
        public ModelPickup(Model model, ItemType type, Vector3 position, Vector3 rotation, Vector3 size = default, bool kinematic = true)
        {
            try
            {
                ItemBase item = Server.InventoryHost.CreateItemInstance(new (type, ItemSerialGenerator.GenerateNext()), false);
                ushort ser = ItemSerialGenerator.GenerateNext();

                item.PickupDropModel.Info.Serial = ser;
                item.PickupDropModel.Info.ItemId = type;
                item.PickupDropModel.Info._serverPosition = model.GameObject.transform.position + position;
                item.PickupDropModel.Info.Weight = item.Weight;
                item.PickupDropModel.Info._serverRotation = Quaternion.Euler(model.GameObject.transform.rotation.eulerAngles + rotation);
                item.PickupDropModel.NetworkInfo = item.PickupDropModel.Info;

                ItemPickupBase ipb = Object.Instantiate(item.PickupDropModel);

                GameObject = ipb.gameObject;
                GameObject.GetComponent<Rigidbody>().isKinematic = kinematic;
                GameObject.transform.parent = model.GameObject.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
                NetworkServer.Spawn(GameObject);

                ipb.InfoReceived(default, item.PickupDropModel.NetworkInfo);
                Pickup = ipb;
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }

        public GameObject GameObject { get; }

        public ItemPickupBase Pickup { get; }
    }
}