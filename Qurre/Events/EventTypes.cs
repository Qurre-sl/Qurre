namespace Qurre.Events;

public static class PlayerEvents //1xxx
{
    //1000+ ← Network events
    public const uint Preauth = 1001;
    public const uint Join = 1002;
    public const uint Leave = 1003;
    public const uint CheckReserveSlot = 1004;
    public const uint CheckWhiteList = 1005;

    //1100+ ← Health events
    public const uint Dead = 1101;
    public const uint Dies = 1102;
    public const uint Damage = 1103;
    public const uint Attack = 1104;
    public const uint Heal = 1105;

    //1200+ ← Admins with Player
    public const uint Ban = 1201;
    public const uint Banned = 1202;
    public const uint Kick = 1203;
    public const uint Mute = 1204;
    public const uint Unmute = 1205;
    public const uint ChangeGroup = 1206;

    //1300+ ← Items
    public const uint CancelUseItem = 1301;
    public const uint UseItem = 1302;
    public const uint UsedItem = 1303;
    public const uint ChangeItem = 1304;
    public const uint UpdateRadio = 1305;
    public const uint UsingRadio = 1306;

    //1400+ ← Pickups
    public const uint PrePickupItem = 1401;
    public const uint PickupItem = 1411;
    public const uint PickupAmmo = 1412;
    public const uint PickupArmor = 1413;
    public const uint PickupCandy = 1414;
    public const uint ThrowProjectile = 1421;
    public const uint DropItem = 1422;
    public const uint DroppedItem = 1423;
    public const uint DropAmmo = 1424;
    public const uint JailbirdTrigger = 1431;

    //1500+ ← Interact
    public const uint InteractDoor = 1501;
    public const uint InteractGenerator = 1502;
    public const uint InteractLift = 1503;
    public const uint InteractLocker = 1504;
    public const uint InteractScp330 = 1505;
    public const uint InteractShootingTarget = 1506;
    public const uint InteractWorkStation = 1507;

    //1600+ ← Gun
    public const uint UnloadGun = 1601;
    public const uint AimGun = 1602;
    public const uint ReloadGun = 1603;
    public const uint ShotGun = 1604;
    public const uint DryFireGun = 1605;
    public const uint GunToggleFlashlight = 1606;

    //1700+ ← Player Role Events
    public const uint Spawn = 1701;
    public const uint ChangeRole = 1702;
    public const uint Escape = 1703;

    //1800+ ← Socialization
    public const uint Cuff = 1801;
    public const uint UnCuff = 1802;
    public const uint ChangeSpectate = 1803;

    //1900+ ← Misc
    public const uint MakeNoise = 1902;
    public const uint ReceiveEffect = 1903;
    public const uint UseHotkey = 1904;
    public const uint PressAlt = 1905;
    public const uint Jump = 1906;

    public const uint DamageWindow = 1951;
    public const uint DamageShootingTarget = 1952;
}

public static class MapEvents //2xxx
{
    //2000+ ← Main Map Events
    public const uint MapGenerated = 2001;
    public const uint LczDecontamination = 2002;
    public const uint LczAnnounce = 2003;

    //2100+ ← Place/Spawn
    public const uint PlaceBlood = 2101;
    public const uint PlaceBulletHole = 2102;
    public const uint CreatePickup = 2103;
    public const uint CorpseSpawn = 2104;
    public const uint CorpseSpawned = 2105;
    public const uint GrenadeExploded = 2106;

    //2200+ ← Team Respawn
    public const uint TeamRespawnSelect = 2201;
    public const uint TeamRespawn = 2202;

    //2300+ ← Objects
    public const uint TriggerTesla = 2301;
    public const uint WorkStationUpdate = 2302;
    public const uint IntercomSetState = 2303;

    //2400+ ← Doors
    public const uint DamageDoor = 2501;
    public const uint LockDoor = 2502;
    public const uint OpenDoor = 2503;
}

public static class ServerEvents //3xxx
{
    //3000+ ← 

    //3100+ ← Commands
    public const uint RequestPlayerListCommand = 3101;
    public const uint RemoteAdminCommand = 3102;
    public const uint GameConsoleCommand = 3103;
    public const uint ServerConsoleCommand = 3104;

    //3200+ ← Reports
    public const uint CheaterReport = 3201;
    public const uint LocalReport = 3202;
}

public static class RoundEvents //4xxx
{
    public const uint Waiting = 4001;
    public const uint Start = 4002;
    public const uint Restart = 4003;
    public const uint End = 4004;
    public const uint Check = 4005;
    public const uint ForceStart = 4006;
}

public static class AlphaEvents //5xxx
{
    public const uint Start = 5001;
    public const uint Stop = 5002;
    public const uint Detonate = 5003;
    public const uint UnlockPanel = 5004;
}

public static class EffectEvents //6xxx
{
    //6000+ ← Main
    public const uint Enabled = 6001;
    public const uint Disabled = 6002;

    //6100+ ← Player
    public const uint Flashed = 6101;
}

public static class ScpEvents //7xxx
{
    //7000+ ← Main
    public const uint Attack = 7001;

    //7100+ ← Scp049
    public const uint Scp049RaisingStart = 7101;
    public const uint Scp049RaisingEnd = 7102;

    //7200+ ← Scp079
    public const uint ActivateGenerator = 7201;
    public const uint Scp079GetExp = 7202;
    public const uint Scp079NewLvl = 7203;
    public const uint Scp079Recontain = 7204;
    public const uint GeneratorStatus = 7205;

    //7300+ ← Scp096
    public const uint Scp096SetState = 7301;
    public const uint Scp096AddTarget = 7302;

    //7400+ ← Scp106
    public const uint Scp106Attack = 7401;

    //7500+ ← Scp173
    public const uint Scp173AddObserver = 7501;
    public const uint Scp173EnableSpeed = 7502;
    public const uint Scp173RemovedObserver = 7503;

    //7600+ ← Scp939


    // Non-game SCPs

    //7700+ ← Scp914
    public const uint Scp914Upgrade = 7701;
    public const uint Scp914UpgradePickup = 7702;
    public const uint Scp914UpgradePlayer = 7703;
}