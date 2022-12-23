﻿namespace Qurre.Events
{
    internal enum PlayerEvents : int //1xxx
    {
        //1000+ <- Network events
        Preauth = 1001,
        Joined = 1002,
        Left = 1003,
        CheckReservedSlot = 1004,

        //1100+ <- Health events
        Death = 1101,

        //1200+ <- Admins with Player
        Banned = 1201,
        Kicked = 1202,
        Muted = 1203,
        Unmuted = 1204,

        //1300+ <- Items
        CancelUsingItem = 1301,
        ChangeItem = 1302,
        UseItem = 1303,
        UsedItem = 1304,
        ChangeRadioRange = 1305,
        ToggleFlashlight = 1306,

        //1400+ <- Pickups
        SearchPickup = 1401,
        SearchedPickup = 1402,
        ThrowItem = 1403,
        DropAmmo = 1404,
        DropItem = 1405,
        PickupAmmo = 1406,
        PickupArmor = 1407,
        PickupScp330 = 1408,


        //1400+ <- Interact
        InteractGenerator = 1401,
        InteractElevator = 1402,
        InteractLocker = 1403,
        InteractScp330 = 1404,
        InteractShootingTarget = 1405,
        UnloadWeapon = 1406,

    }

    internal enum MapEvents : int //2xxx
    {

    }
}