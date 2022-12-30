using InventorySystem.Items.MicroHID;
using Qurre.API.Controllers;

namespace Qurre.API.Addons.Items
{
    public sealed class MicroHID : Item
    {
        private const ItemType MicroHIDItemType = ItemType.MicroHID;

        public new MicroHIDItem Base { get; }

        public float Energy
        {
            get => Base.RemainingEnergy;
            set => Base.RemainingEnergy = value;
        }

        /// <summary>
        /// 0 - 255
        /// </summary>
        public byte EnergyPercent
        {
            get => Base.EnergyToByte;
            set
            {
                Base.RemainingEnergy = value / 225f;
            }
        }

        public HidState State
        {
            get => Base.State;
            set => Base.State = value;
        }

        public MicroHID(MicroHIDItem itemBase) : base(itemBase)
        {
            Base = itemBase;
        }

        public MicroHID() : this((MicroHIDItem)MicroHIDItemType.CreateItemInstance())
        {
        }

        public void Fire()
        {
            Base.UserInput = HidUserInput.Fire;
            State = HidState.Firing;
        }
    }
}