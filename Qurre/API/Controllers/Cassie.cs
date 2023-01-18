using Respawning;

namespace Qurre.API.Controllers
{
    public class Cassie
    {
        public Cassie(string message, bool makeHold, bool makeNoise)
        {
            Message = message;
            Hold = makeHold;
            Noise = makeNoise;
        }

        public static bool Lock { get; set; }

        public string Message { get; set; }
        public bool Hold { get; set; }
        public bool Noise { get; set; }
        public bool Active { get; private set; }

        public static void Send(string msg, bool makeHold = false, bool makeNoise = false, bool instant = false) =>
            Map.Cassies.Add(new (msg, makeHold, makeNoise), instant);

        public void Send()
        {
            if (Active)
            {
                return;
            }

            Active = true;
            RespawnEffectsController.PlayCassieAnnouncement(Message, Hold, Noise);
        }

        internal static void End()
        {
            if (Map.Cassies.Count > 0)
            {
                Map.Cassies[0].Send();
            }
        }
    }
}