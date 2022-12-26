namespace Qurre.Events
{
    static public class PlayerEvents //1xxx
    {
        //1000+ <- Network events
        public const uint Preauth = 1001;
        public const uint Join = 1002;
        public const uint Leave = 1003;
        public const uint CheckReserveSlot = 1004;

        //1100+ <- Health events
        public const uint Dead = 1101;
        public const uint Dies = 1102;
        public const uint Damage = 1103;

        //1200+ <- Admins with Player
        public const uint Banned = 1201;
        public const uint Kicked = 1202;
        public const uint Muted = 1203;
        public const uint Unmuted = 1204;

        //1300+ <- Items
        public const uint CancelUsingItem = 1301;
        public const uint ChangeItem = 1302;
        public const uint UseItem = 1303;
        public const uint UsedItem = 1304;
        public const uint ChangeRadioRange = 1305;
        public const uint ToggleFlashlight = 1306;

        //1400+ <- Pickups
        public const uint SearchPickup = 1401;
        public const uint SearchedPickup = 1402;
        public const uint ThrowItem = 1403;
        public const uint DropAmmo = 1404;
        public const uint DropItem = 1405;
        public const uint PickupAmmo = 1406;
        public const uint PickupArmor = 1407;
        public const uint PickupScp330 = 1408;

        //1500+ <- Interact
        public const uint InteractGenerator = 1501;
        public const uint InteractElevator = 1502;
        public const uint InteractLocker = 1503;
        public const uint InteractScp330 = 1504;
        public const uint InteractShootingTarget = 1505;

        //1600+ <- Gun
        public const uint UnloadGun = 1601;
        public const uint AimGun = 1602;
        public const uint ReloadGun = 1603;
        public const uint ShotGun = 1604;
        public const uint DryfireGun = 1605;

        //1700+ <- Player Role Events
        public const uint Spawn = 1701;
        public const uint ChangeRole = 1702;
        public const uint Escape = 1703;

        //1800+ <- Damage objects
        public const uint DamageWindow = 1801;
        public const uint DamageShootingTarget = 1802;

        //1900+ <- Misc
        public const uint ChangeSpectator = 1901;
        public const uint Handcuff = 1902;
        public const uint RemoveHandcuffs = 1903;
        public const uint MakeNoise = 1904;
        public const uint ReceiveEffect = 1905;
        public const uint UseHotkey = 1906;
    }

    static public class MapEvents //2xxx
    {
        //2000+ <- Main Map Events
        public const uint MapGenerated = 2001;
        public const uint LczDecontamination = 2002;
        public const uint LczAnnounce = 2003;
        public const uint GeneratorActivated = 2004;

        //2100+ <- Place/Spawn
        public const uint PlaceBlood = 2101;
        public const uint PlaceBulletHole = 2102;
        public const uint ItemSpawned = 2103;
        public const uint RagdollSpawn = 2104;
        public const uint GrenadeExploded = 2105;

        //2200+ <- Team Respawn
        public const uint TeamRespawnSelected = 2201;
        public const uint TeamRespawn = 2202;
    }

    static public class ServerEvents //3xxx
    {
        //3000+ <- 

        //3100+ <- Commands
        public const uint RemoteAdminCommand = 3101;
        public const uint GameConsoleCommand = 3102;
        public const uint ServerConsoleCommand = 3103;

        //3200+ <- Reports
        public const uint CheaterReport = 3201;
        public const uint LocalReport = 3202;
    }

    static public class RoundEvents //4xxx
    {
        public const uint Waiting = 4001;
        public const uint Start = 4002;
        public const uint Restart = 4003;
        public const uint End = 4004;
    }

    static public class AlphaEvents //5xxx
    {
        public const uint Start = 5001;
        public const uint Stop = 5002;
        public const uint Detonation = 5003;
    }

    static public class ScpEvents //6xxx
    {

    }
}