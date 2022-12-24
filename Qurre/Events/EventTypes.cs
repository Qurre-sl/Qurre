namespace Qurre.Events
{
    public enum PlayerEvents : uint //1xxx
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

        //1500+ <- Interact
        InteractGenerator = 1501,
        InteractElevator = 1502,
        InteractLocker = 1503,
        InteractScp330 = 1504,
        InteractShootingTarget = 1505,

        //1600+ <- Gun
        UnloadGun = 1601,
        AimGun = 1602,
        ReloadGun = 1603,
        ShotGun = 1604,
        DryfireGun = 1605,

        //1700+ <- Player Role Events
        Spawn = 1701,
        ChangeRole = 1702,
        Escape = 1703,

        //1800+ <- Damage
        Damage = 1801,
        DamageWindow = 1802,
        DamageShootingTarget = 1803,

        //1900+ <- Misc
        ChangeSpectator = 1901,
        Handcuff = 1902,
        RemoveHandcuffs = 1903,
        MakeNoise = 1904,
        ReceiveEffect = 1905,
        UseHotkey = 1906,
    }

    public enum MapEvents : uint //2xxx
    {
        //2000+ <- Main Map Events
        MapGenerated = 2001,
        LczDecontamination = 2002,
        LczAnnounce = 2003,
        GeneratorActivated = 2004,

        //2100+ <- Place/Spawn
        PlaceBlood = 2101,
        PlaceBulletHole = 2102,
        ItemSpawned = 2103,
        RagdollSpawn = 2104,
        GrenadeExploded = 2105,

        //2200+ <- Team Respawn
        TeamRespawnSelected = 2201,
        TeamRespawn = 2202,
    }

    public enum ServerEvents : uint //3xxx
    {
        //3000+ <- 

        //3100+ <- Commands
        RemoteAdminCommand = 3101,
        GameConsoleCommand = 3102,
        ServerConsoleCommand = 3103,

        //3200+ <- Reports
        CheaterReport = 3201,
        LocalReport = 3202,
    }

    public enum RoundEvents : uint //4xxx
    {
        Waiting = 4001,
        Start = 4002,
        Restart = 4003,
        End = 4004,
    }
    public enum AlphaEvents : uint //5xxx
    {
        Start = 5001,
        Stop = 5002,
        Detonation = 5003,
    }
}