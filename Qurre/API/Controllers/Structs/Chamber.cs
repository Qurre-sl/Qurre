﻿using System.Collections.Generic;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Pickups;
using MapGeneration.Distributors;

namespace Qurre.API.Controllers.Structs
{
    public class Chamber
    {
        internal Chamber(LockerChamber _chamber, Locker _locker)
        {
            LockerChamber = _chamber;
            Locker = _locker;
        }

        public LockerChamber LockerChamber { get; }
        public Locker Locker { get; }

        public HashSet<ItemPickupBase> ToBeSpawned => LockerChamber._toBeSpawned;

        public bool CanInteract => LockerChamber.CanInteract;

        public bool Open
        {
            get => LockerChamber.IsOpen;
            set => LockerChamber.SetDoor(value, Locker.GrantedBeep);
        }

        public ItemType[] AcceptableItems
        {
            get => LockerChamber.AcceptableItems;
            set => LockerChamber.AcceptableItems = value;
        }

        public float Cooldown
        {
            get => LockerChamber._targetCooldown;
            set => LockerChamber._targetCooldown = value;
        }

        public KeycardPermissions Permissions
        {
            get => LockerChamber.RequiredPermissions;
            set => LockerChamber.RequiredPermissions = value;
        }

        public void SpawnItem(ItemType id, int amount) => LockerChamber.SpawnItem(id, amount);
    }
}