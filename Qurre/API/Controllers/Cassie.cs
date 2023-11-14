using Respawning;

namespace Qurre.API.Controllers
{
    public class Cassie
    {
        static public bool Lock { get; set; }

        public string Message { get; set; }
        public bool Hold { get; set; }
        public bool Noise { get; set; }
        public bool Active { get; private set; }

        public void Send()
        {
            if (Active) return;
            Active = true;
            RespawnEffectsController.PlayCassieAnnouncement(Message, Hold, Noise);
        }

        static public void Send(string msg, bool makeHold = false, bool makeNoise = false, bool instant = false) =>
            Map.Cassies.Add(new Cassie(msg, makeHold, makeNoise), instant);

        static internal void End()
        {
            if (Map.Cassies.Count > 0) Map.Cassies[0].Send();
        }

        public Cassie(string message, bool makeHold, bool makeNoise)
        {
            Message = message;
            Hold = makeHold;
            Noise = makeNoise;
            Active = false;
        }
    }
}