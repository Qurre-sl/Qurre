using Qurre.API.Controllers.Structs;
using MEC;

namespace Qurre.API.Controllers
{
    public class Broadcast
    {
        private readonly Player pl;
        private string msg;

        public float DisplayTime { get; internal set; } = float.MinValue;
        public ushort Time { get; }
        public bool Active { get; private set; }
        public string Message
        {
            get => msg;
            set
            {
                if (value != msg)
                {
                    msg = value;
                    if (Active) Update();
                }
            }
        }

        public void Start()
        {
            if (pl.Broadcasts.FirstOrDefault() != this) return;
            if (Active) return;
            Active = true;
            DisplayTime = UnityEngine.Time.time;
            BcComponent.Component.TargetAddElement(pl.Connection, Message, Time, global::Broadcast.BroadcastFlags.Normal);
            Timing.CallDelayed(Time, () => End());
        }
        public void Update()
        {
            var time = Time - (UnityEngine.Time.time - DisplayTime) + 1;
            BcComponent.Component.TargetClearElements(pl.Connection);
            BcComponent.Component.TargetAddElement(pl.Connection, Message, (ushort)time, global::Broadcast.BroadcastFlags.Normal);
        }
        public void End()
        {
            if (!Active) return;
            Active = false;
            pl.Broadcasts.Remove(this);
            BcComponent.Component.TargetClearElements(pl.Connection);
            if (pl.Broadcasts.FirstOrDefault() != null) pl.Broadcasts.FirstOrDefault().Start();
        }

        internal void SmallEnd()
        {
            Active = false;
            BcComponent.Component.TargetClearElements(pl.Connection);
        }

        public Broadcast(Player player, string message, ushort time)
        {
            Message = message;
            Time = time;
            pl = player;
        }
    }
}