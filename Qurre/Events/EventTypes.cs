namespace Qurre.Events
{
    internal enum PlayerEvents : int
    {
        //1000+ <- Network events
        Preauth = 1000,
        Joined = 1001,
        Left = 1002,

        //1100+ <- Health events
        Death = 1101,

        //1200+ <- Admins with Player
        Banned = 1201,
        Kicked = 1202,

        //1300+ <- Items
        CancelUsingItem = 1301,
        ChangeItem = 1302,
        DropAmmo = 1303,
        DropItem = 1304,
        PickupAmmo = 1305,
        PickupArmor = 1306,
        PickupScp330 = 1307,
        ChangeRadioRange = 1307,


        //1400+ <- Interact
        InteractGenerator = 1401,
        InteractElevator = 1402,
        InteractLocker = 1403,
        InteractScp330 = 1404,
        InteractShootingTarget = 1405,

    }

    internal enum MapEvents : int
    {

    }
}