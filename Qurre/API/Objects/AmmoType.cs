namespace Qurre.API.Objects
{
    public enum AmmoType : byte
    {
        None,

        /// <summary>
        ///     Used by <see cref="ItemType.GunE11SR" />
        /// </summary>
        Ammo556,

        /// <summary>
        ///     Used by and <see cref="ItemType.GunLogicer" />, <see cref="ItemType.GunAK" />
        /// </summary>
        Ammo762,

        /// <summary>
        ///     Used by <see cref="ItemType.GunCOM15" />, <see cref="ItemType.GunCOM18" />, <see cref="ItemType.GunCrossvec" />,
        ///     <see cref="ItemType.GunFSP9" />
        /// </summary>
        Ammo9,

        /// <summary>
        ///     Used by <see cref="ItemType.GunShotgun" />
        /// </summary>
        Ammo12Gauge,

        /// <summary>
        ///     Used by <see cref="ItemType.GunRevolver" />.
        /// </summary>
        Ammo44Cal
    }
}