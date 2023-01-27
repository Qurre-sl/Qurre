using MEC;
using System.Collections.Generic;
using System.Linq;

namespace Qurre.API.Controllers
{
    public class MapBroadcast
    {
        private string msg;
        private readonly List<Broadcast> list = new();

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
                    foreach (var bc in list) bc.Message = value;
                }
            }
        }

        public void Start()
        {
            if (Active) return;
            Active = true;

            Timing.CallDelayed(Time, () => End());
        }
        public void End()
        {
            if (!Active) return;
            Active = false;

            foreach (var bc in list)
                bc.End();
        }

        public MapBroadcast(string message, ushort time, bool instant, bool adminBC)
        {
            msg = message;
            Time = time;

            Start();

            if (adminBC)
            {
                foreach (Player pl in Player.List.Where(x => PermissionsHandler.IsPermitted(x.Sender.Permissions, PlayerPermissions.AdminChat)))
                {
                    var bc = pl.Client.Broadcast(message, time, instant);
                    list.Add(bc);
                }
            }
            else
            {
                foreach (Player pl in Player.List)
                {
                    var bc = pl.Client.Broadcast(message, time, instant);
                    list.Add(bc);
                }
            }
        }
    }
}